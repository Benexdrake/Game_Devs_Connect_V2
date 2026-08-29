using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests;

internal static class QuestAccess
{
    public static async Task<bool> CanViewProjectQuestsAsync(AppDbContext db, Project project, Guid? userId, CancellationToken ct)
    {
        if (project.Visibility == ProjectVisibility.Public)
        {
            return true;
        }

        return userId is not null &&
            await db.ProjectMembers.AnyAsync(m => m.ProjectId == project.Id && m.UserId == userId, ct);
    }
}
