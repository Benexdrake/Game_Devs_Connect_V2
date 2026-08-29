using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects;

internal static class ProjectAccess
{
    public static Task<ProjectRole?> GetRoleAsync(AppDbContext db, Guid projectId, Guid userId, CancellationToken ct) =>
        db.ProjectMembers
            .Where(m => m.ProjectId == projectId && m.UserId == userId)
            .Select(m => (ProjectRole?)m.Role)
            .FirstOrDefaultAsync(ct);

    public static Task<int> CountOwnersAsync(AppDbContext db, Guid projectId, CancellationToken ct) =>
        db.ProjectMembers.CountAsync(m => m.ProjectId == projectId && m.Role == ProjectRole.Owner, ct);
}
