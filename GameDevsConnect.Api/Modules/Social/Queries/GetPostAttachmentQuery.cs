using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Quests;
using GameDevsConnect.Api.Shared;
using GameDevsConnect.Api.Shared.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Queries;

public record PostAttachmentContent(Stream Content, string ContentType, string FileName);

public record GetPostAttachmentQuery(Guid PostId, Guid AttachmentId, Guid? RequestingUserId) : IRequest<Result<PostAttachmentContent>>;

public class GetPostAttachmentQueryHandler(AppDbContext db, IFileStorage fileStorage)
    : IRequestHandler<GetPostAttachmentQuery, Result<PostAttachmentContent>>
{
    public async Task<Result<PostAttachmentContent>> Handle(GetPostAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await db.PostAttachments.FirstOrDefaultAsync(
            a => a.Id == request.AttachmentId && a.PostId == request.PostId, cancellationToken);
        if (attachment is null)
        {
            return Result<PostAttachmentContent>.NotFound("Attachment not found.");
        }

        var post = await db.Posts.FirstAsync(p => p.Id == request.PostId, cancellationToken);
        var project = await db.Projects.FirstAsync(p => p.Id == post.ProjectId, cancellationToken);

        if (!await QuestAccess.CanViewProjectQuestsAsync(db, project, request.RequestingUserId, cancellationToken))
        {
            return Result<PostAttachmentContent>.NotFound("Attachment not found.");
        }

        var stream = fileStorage.OpenRead(attachment.StoragePath);
        return Result<PostAttachmentContent>.Success(new PostAttachmentContent(stream, attachment.ContentType, attachment.FileName));
    }
}
