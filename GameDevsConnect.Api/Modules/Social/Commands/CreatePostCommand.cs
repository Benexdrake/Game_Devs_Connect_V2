using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Social.Events;
using GameDevsConnect.Api.Shared;
using GameDevsConnect.Api.Shared.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Commands;

public record CreatePostCommand(
    string ProjectSlug,
    Guid RequestingUserId,
    string Body,
    IReadOnlyList<UploadedFileInput> Attachments) : IRequest<Result<PostDto>>;

public class CreatePostCommandHandler(AppDbContext db, IFileStorage fileStorage, IMediator mediator)
    : IRequestHandler<CreatePostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return Result<PostDto>.ValidationError("Body is required.");
        }

        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.ProjectSlug, cancellationToken);
        if (project is null)
        {
            return Result<PostDto>.NotFound("Project not found.");
        }

        var isMember = await db.ProjectMembers.AnyAsync(
            m => m.ProjectId == project.Id && m.UserId == request.RequestingUserId, cancellationToken);
        if (!isMember)
        {
            return Result<PostDto>.Forbidden("Only project members can post updates.");
        }

        var now = DateTimeOffset.UtcNow;
        var post = new Post
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            AuthorId = request.RequestingUserId,
            Body = request.Body,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false,
        };
        db.Posts.Add(post);

        foreach (var file in request.Attachments)
        {
            var safeName = Path.GetFileName(file.FileName);
            var attachmentId = Guid.NewGuid();
            var storagePath = $"posts/{post.Id}/{attachmentId}-{safeName}";

            await fileStorage.SaveAsync(storagePath, file.Content, cancellationToken);

            db.PostAttachments.Add(new PostAttachment
            {
                Id = attachmentId,
                PostId = post.Id,
                FileName = safeName,
                ContentType = file.ContentType,
                StoragePath = storagePath,
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new PostCreatedEvent(post.Id, project.Id, request.RequestingUserId), cancellationToken);

        return Result<PostDto>.Success(await PostDtoBuilder.BuildAsync(db, post, request.RequestingUserId, cancellationToken));
    }
}
