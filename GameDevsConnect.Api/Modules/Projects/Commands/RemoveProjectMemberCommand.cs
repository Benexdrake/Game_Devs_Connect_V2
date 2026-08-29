using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects.Commands;

public record RemoveProjectMemberCommand(
    string Slug,
    Guid RequestingUserId,
    string TargetUsername) : IRequest<Result<bool>>;

public class RemoveProjectMemberCommandHandler(AppDbContext db)
    : IRequestHandler<RemoveProjectMemberCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);
        if (project is null)
        {
            return Result<bool>.NotFound("Project not found.");
        }

        var targetUser = await db.Users.FirstOrDefaultAsync(u => u.Username == request.TargetUsername, cancellationToken);
        if (targetUser is null)
        {
            return Result<bool>.NotFound("User not found.");
        }

        var member = await db.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == project.Id && m.UserId == targetUser.Id, cancellationToken);
        if (member is null)
        {
            return Result<bool>.NotFound("This user is not a member of the project.");
        }

        var isSelf = targetUser.Id == request.RequestingUserId;
        if (!isSelf)
        {
            var requesterRole = await ProjectAccess.GetRoleAsync(db, project.Id, request.RequestingUserId, cancellationToken);
            if (requesterRole is not (ProjectRole.Owner or ProjectRole.Admin))
            {
                return Result<bool>.Forbidden("Only the owner or an admin can remove members.");
            }
        }

        if (member.Role == ProjectRole.Owner)
        {
            var ownerCount = await ProjectAccess.CountOwnersAsync(db, project.Id, cancellationToken);
            if (ownerCount <= 1)
            {
                return Result<bool>.Conflict(
                    "Cannot remove the only owner. Promote another member to owner first.");
            }
        }

        db.ProjectMembers.Remove(member);
        await db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
