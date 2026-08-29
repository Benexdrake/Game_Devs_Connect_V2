using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Queries;

// "For you": activity tied to the topics the requesting user picked as
// skills, rather than to who they follow (that's GetFeedQuery). Two match
// paths, both restricted to projects they can actually see:
//  - QuestCreated/ContributionAccepted whose quest's SkillCategory is one
//    they have a skill in.
//  - ProjectPosted from a project whose engine matches one of their
//    "Engines" category skills (Unity skill -> Unity-engine projects).
// No signal exists to place MemberJoined/LevelUp here, so they never show up
// in this tab - they still appear in Following if the actor is followed.
public record GetForYouFeedQuery(Guid RequestingUserId, int Page, int PageSize) : IRequest<Result<IReadOnlyList<ActivityEventDto>>>;

public class GetForYouFeedQueryHandler(AppDbContext db) : IRequestHandler<GetForYouFeedQuery, Result<IReadOnlyList<ActivityEventDto>>>
{
    public async Task<Result<IReadOnlyList<ActivityEventDto>>> Handle(GetForYouFeedQuery request, CancellationToken cancellationToken)
    {
        var userSkills = await db.UserSkills
            .Where(us => us.UserId == request.RequestingUserId)
            .Join(db.Skills, us => us.SkillId, s => s.Id, (us, s) => s)
            .ToListAsync(cancellationToken);

        var categories = userSkills.Select(s => s.Category).Distinct().ToList();
        var engineNames = userSkills.Where(s => s.Category == SkillCategory.Engines).Select(s => s.Name).ToList();

        var matchingQuestIds = await (
            from q in db.Quests
            join p in db.Projects on q.ProjectId equals p.Id
            where categories.Contains(q.Category) &&
                (p.Visibility == ProjectVisibility.Public ||
                    db.ProjectMembers.Any(m => m.ProjectId == p.Id && m.UserId == request.RequestingUserId))
            select q.Id
        ).ToListAsync(cancellationToken);

        var matchingProjectIds = await (
            from p in db.Projects
            join e in db.Engines on p.EngineId equals e.Id
            where engineNames.Contains(e.Name) &&
                (p.Visibility == ProjectVisibility.Public ||
                    db.ProjectMembers.Any(m => m.ProjectId == p.Id && m.UserId == request.RequestingUserId))
            select p.Id
        ).ToListAsync(cancellationToken);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var events = await db.ActivityEvents
            .Where(e =>
                (e.QuestId != null && matchingQuestIds.Contains(e.QuestId.Value)) ||
                (e.Type == ActivityEventType.ProjectPosted && e.ProjectId != null && matchingProjectIds.Contains(e.ProjectId.Value)))
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = await GetFeedQueryHandler.BuildDtosAsync(db, events, cancellationToken);
        return Result<IReadOnlyList<ActivityEventDto>>.Success(dtos);
    }
}
