using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects.Commands;

public record AddProjectMemberCommand(
    string Slug,
    Guid RequestingUserId,
    string Username,
    ProjectRole Role) : IRequest<Result<ProjectMemberDto>>;

public class AddProjectMemberCommandHandler(AppDbContext db)
    : IRequestHandler<AddProjectMemberCommand, Result<ProjectMemberDto>>
{
    public async Task<Result<ProjectMemberDto>> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);
        if (project is null)
        {
            return Result<ProjectMemberDto>.NotFound("Project not found.");
        }

        var requesterRole = await ProjectAccess.GetRoleAsync(db, project.Id, request.RequestingUserId, cancellationToken);
        if (requesterRole is not (ProjectRole.Owner or ProjectRole.Admin))
        {
            return Result<ProjectMemberDto>.Forbidden("Only the owner or an admin can invite members.");
        }

        if (requesterRole == ProjectRole.Admin && request.Role != ProjectRole.Contributor)
        {
            return Result<ProjectMemberDto>.Forbidden("Admins can only invite contributors.");
        }

        var targetUser = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        if (targetUser is null)
        {
            return Result<ProjectMemberDto>.NotFound("User not found.");
        }

        var alreadyMember = await db.ProjectMembers
            .AnyAsync(m => m.ProjectId == project.Id && m.UserId == targetUser.Id, cancellationToken);
        if (alreadyMember)
        {
            return Result<ProjectMemberDto>.Conflict("User is already a member of this project.");
        }

        var member = new ProjectMember
        {
            ProjectId = project.Id,
            UserId = targetUser.Id,
            Role = request.Role,
            JoinedAt = DateTimeOffset.UtcNow,
        };
        db.ProjectMembers.Add(member);
        await db.SaveChangesAsync(cancellationToken);

        return Result<ProjectMemberDto>.Success(
            new ProjectMemberDto(targetUser.Id, targetUser.Username, targetUser.AvatarUrl, member.Role));
    }
}
