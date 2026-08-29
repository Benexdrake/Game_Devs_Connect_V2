using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Commands;

public record DeletePostCommand(Guid PostId, Guid RequestingUserId) : IRequest<Result<bool>>;

public class DeletePostCommandHandler(AppDbContext db) : IRequestHandler<DeletePostCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId && !p.IsDeleted, cancellationToken);
        if (post is null)
        {
            return Result<bool>.NotFound("Post not found.");
        }

        if (post.AuthorId != request.RequestingUserId)
        {
            var role = await ProjectAccess.GetRoleAsync(db, post.ProjectId, request.RequestingUserId, cancellationToken);
            if (role is not (ProjectRole.Owner or ProjectRole.Admin))
            {
                return Result<bool>.Forbidden("Only the author or an owner/admin can delete this post.");
            }
        }

        post.IsDeleted = true;
        post.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
