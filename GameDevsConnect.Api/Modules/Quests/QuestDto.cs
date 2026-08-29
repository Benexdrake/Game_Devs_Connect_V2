using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests;

public record QuestSkillDto(Guid Id, string Name, SkillCategory Category);

public record QuestDto(
    Guid Id,
    Guid ProjectId,
    string ProjectSlug,
    string ProjectTitle,
    Guid CreatorId,
    string CreatorUsername,
    string Title,
    string? Description,
    SkillCategory Category,
    QuestDifficulty Difficulty,
    int XpReward,
    QuestStatus Status,
    DateTimeOffset? Deadline,
    int MaxContributors,
    Guid? ClaimedByUserId,
    string? ClaimedByUsername,
    IReadOnlyList<QuestSkillDto> RequiredSkills,
    DateTimeOffset CreatedAt);

internal static class QuestDtoBuilder
{
    public static async Task<QuestDto> BuildAsync(AppDbContext db, Quest quest, CancellationToken ct)
    {
        var project = await db.Projects.FirstAsync(p => p.Id == quest.ProjectId, ct);
        var creator = await db.Users.FirstAsync(u => u.Id == quest.CreatorId, ct);

        var skills = await db.QuestSkills
            .Where(qs => qs.QuestId == quest.Id)
            .Join(db.Skills, qs => qs.SkillId, s => s.Id, (qs, s) => new QuestSkillDto(s.Id, s.Name, s.Category))
            .ToListAsync(ct);

        var claimer = await db.QuestAssignments
            .Where(a => a.QuestId == quest.Id && a.ReleasedAt == null)
            .Join(db.Users, a => a.UserId, u => u.Id, (a, u) => new { u.Id, u.Username })
            .FirstOrDefaultAsync(ct);

        return new QuestDto(
            quest.Id,
            project.Id,
            project.Slug,
            project.Title,
            creator.Id,
            creator.Username,
            quest.Title,
            quest.Description,
            quest.Category,
            quest.Difficulty,
            quest.XpReward,
            quest.Status,
            quest.Deadline,
            quest.MaxContributors,
            claimer?.Id,
            claimer?.Username,
            skills,
            quest.CreatedAt);
    }
}
