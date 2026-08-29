using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests.Queries;

public record GetQuestsQuery(
    string? Search,
    SkillCategory? Category,
    Guid? SkillId,
    string? ProjectSlug,
    QuestDifficulty? Difficulty,
    int? MinXp,
    Guid? EngineId,
    Guid? RequestingUserId) : IRequest<Result<IReadOnlyList<QuestDto>>>;

public class GetQuestsQueryHandler(AppDbContext db) : IRequestHandler<GetQuestsQuery, Result<IReadOnlyList<QuestDto>>>
{
    public async Task<Result<IReadOnlyList<QuestDto>>> Handle(GetQuestsQuery request, CancellationToken cancellationToken)
    {
        var query =
            from q in db.Quests
            join p in db.Projects on q.ProjectId equals p.Id
            join u in db.Users on q.CreatorId equals u.Id
            where q.Status == QuestStatus.Open
            select new { Quest = q, Project = p, Creator = u };

        query = query.Where(x => x.Project.Visibility == ProjectVisibility.Public ||
            (request.RequestingUserId != null &&
                db.ProjectMembers.Any(m => m.ProjectId == x.Project.Id && m.UserId == request.RequestingUserId)));

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x => EF.Functions.ILike(x.Quest.Title, $"%{request.Search}%"));
        }

        if (request.Category is not null)
        {
            query = query.Where(x => x.Quest.Category == request.Category);
        }

        if (request.ProjectSlug is not null)
        {
            query = query.Where(x => x.Project.Slug == request.ProjectSlug);
        }

        if (request.Difficulty is not null)
        {
            query = query.Where(x => x.Quest.Difficulty == request.Difficulty);
        }

        if (request.MinXp is not null)
        {
            query = query.Where(x => x.Quest.XpReward >= request.MinXp);
        }

        if (request.EngineId is not null)
        {
            query = query.Where(x => x.Project.EngineId == request.EngineId);
        }

        if (request.SkillId is not null)
        {
            var questIdsWithSkill = db.QuestSkills.Where(qs => qs.SkillId == request.SkillId).Select(qs => qs.QuestId);
            query = query.Where(x => questIdsWithSkill.Contains(x.Quest.Id));
        }

        var rows = await query.OrderByDescending(x => x.Quest.CreatedAt).ToListAsync(cancellationToken);

        var questIds = rows.Select(r => r.Quest.Id).ToList();
        var skillsByQuest = await db.QuestSkills
            .Where(qs => questIds.Contains(qs.QuestId))
            .Join(db.Skills, qs => qs.SkillId, s => s.Id,
                (qs, s) => new { qs.QuestId, Skill = new QuestSkillDto(s.Id, s.Name, s.Category) })
            .ToListAsync(cancellationToken);

        var activeClaimsByQuest = await db.QuestAssignments
            .Where(a => questIds.Contains(a.QuestId) && a.ReleasedAt == null)
            .Join(db.Users, a => a.UserId, u => u.Id, (a, u) => new { a.QuestId, u.Id, u.Username })
            .ToDictionaryAsync(x => x.QuestId, cancellationToken);

        var dtos = rows.Select(r =>
        {
            var claimer = activeClaimsByQuest.GetValueOrDefault(r.Quest.Id);
            return new QuestDto(
                r.Quest.Id,
                r.Project.Id,
                r.Project.Slug,
                r.Project.Title,
                r.Creator.Id,
                r.Creator.Username,
                r.Quest.Title,
                r.Quest.Description,
                r.Quest.Category,
                r.Quest.Difficulty,
                r.Quest.XpReward,
                r.Quest.Status,
                r.Quest.Deadline,
                r.Quest.MaxContributors,
                claimer?.Id,
                claimer?.Username,
                skillsByQuest.Where(s => s.QuestId == r.Quest.Id).Select(s => s.Skill).ToList(),
                r.Quest.CreatedAt);
        }).ToList();

        return Result<IReadOnlyList<QuestDto>>.Success(dtos);
    }
}
