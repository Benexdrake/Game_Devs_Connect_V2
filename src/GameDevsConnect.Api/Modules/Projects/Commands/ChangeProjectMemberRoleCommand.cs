using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects.Commands;

public record ChangeProjectMemberRoleCommand(
    string Slug,
    Guid RequestingUserId,
    string TargetUsername,
    ProjectRole NewRole) : IRequest<Result<ProjectMemberDto>>;

public class ChangeProjectMemberRoleCommandHandler(AppDbContext db)
    : IRequestHandler<ChangeProjectMemberRoleCommand, Result<ProjectMemberDto>>
{
    public async Task<Result<ProjectMemberDto>> Handle(ChangeProjectMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);
        if (project is null)
        {
            return Result<ProjectMemberDto>.NotFound("Project not found.");
        }

        var requesterRole = await ProjectAccess.GetRoleAsync(db, project.Id, request.RequestingUserId, cancellationToken);
        if (requesterRole is not ProjectRole.Owner)
        {
            return Result<ProjectMemberDto>.Forbidden("Only the owner can change member roles.");
        }

        var targetUser = await db.Users.FirstOrDefaultAsync(u => u.Username == request.TargetUsername, cancellationToken);
        if (targetUser is null)
        {
            return Result<ProjectMemberDto>.NotFound("User not found.");
        }

        var member = await db.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == project.Id && m.UserId == targetUser.Id, cancellationToken);
        if (member is null)
        {
            return Result<ProjectMemberDto>.NotFound("This user is not a member of the project.");
        }

        if (member.Role == ProjectRole.Owner && request.NewRole != ProjectRole.Owner)
        {
            var ownerCount = await ProjectAccess.CountOwnersAsync(db, project.Id, cancellationToken);
            if (ownerCount <= 1)
            {
                return Result<ProjectMemberDto>.Conflict(
                    "Cannot demote the only owner. Promote another member to owner first.");
            }
        }

        member.Role = request.NewRole;
        await db.SaveChangesAsync(cancellationToken);

        return Result<ProjectMemberDto>.Success(
            new ProjectMemberDto(targetUser.Id, targetUser.Username, targetUser.AvatarUrl, member.Role));
    }
}
