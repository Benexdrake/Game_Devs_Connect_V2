using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Commands;

public record DeleteCommentCommand(Guid CommentId, Guid RequestingUserId) : IRequest<Result<bool>>;

public class DeleteCommentCommandHandler(AppDbContext db) : IRequestHandler<DeleteCommentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await db.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId && !c.IsDeleted, cancellationToken);
        if (comment is null)
        {
            return Result<bool>.NotFound("Comment not found.");
        }

        if (comment.AuthorId != request.RequestingUserId)
        {
            var post = await db.Posts.FirstAsync(p => p.Id == comment.PostId, cancellationToken);
            var role = await ProjectAccess.GetRoleAsync(db, post.ProjectId, request.RequestingUserId, cancellationToken);
            if (role is not (ProjectRole.Owner or ProjectRole.Admin))
            {
                return Result<bool>.Forbidden("Only the author or an owner/admin can delete this comment.");
            }
        }

        comment.IsDeleted = true;
        await db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
