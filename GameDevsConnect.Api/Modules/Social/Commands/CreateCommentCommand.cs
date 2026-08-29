using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Commands;

public record CreateCommentCommand(Guid PostId, Guid RequestingUserId, string Body) : IRequest<Result<CommentDto>>;

public class CreateCommentCommandHandler(AppDbContext db) : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return Result<CommentDto>.ValidationError("Body is required.");
        }

        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);
        if (post is null)
        {
            return Result<CommentDto>.NotFound("Post not found.");
        }

        var isMember = await db.ProjectMembers.AnyAsync(
            m => m.ProjectId == post.ProjectId && m.UserId == request.RequestingUserId, cancellationToken);
        if (!isMember)
        {
            return Result<CommentDto>.Forbidden("Only project members can comment.");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            AuthorId = request.RequestingUserId,
            Body = request.Body,
            CreatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false,
        };
        db.Comments.Add(comment);
        await db.SaveChangesAsync(cancellationToken);

        var author = await db.Users.FirstAsync(u => u.Id == request.RequestingUserId, cancellationToken);
        return Result<CommentDto>.Success(new CommentDto(comment.Id, author.Id, author.Username, comment.Body, comment.CreatedAt));
    }
}
