using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Queries;

public record GetFeedQuery(Guid RequestingUserId, int Page, int PageSize) : IRequest<Result<IReadOnlyList<ActivityEventDto>>>;

public class GetFeedQueryHandler(AppDbContext db) : IRequestHandler<GetFeedQuery, Result<IReadOnlyList<ActivityEventDto>>>
{
    public async Task<Result<IReadOnlyList<ActivityEventDto>>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var followedUserIds = await db.Follows
            .Where(f => f.FollowerUserId == request.RequestingUserId && f.TargetType == FollowTargetType.User)
            .Select(f => f.TargetId)
            .ToListAsync(cancellationToken);

        var followedProjectIds = await db.Follows
            .Where(f => f.FollowerUserId == request.RequestingUserId && f.TargetType == FollowTargetType.Project)
            .Select(f => f.TargetId)
            .ToListAsync(cancellationToken);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var events = await db.ActivityEvents
            .Where(e => followedUserIds.Contains(e.ActorUserId) ||
                (e.ProjectId != null && followedProjectIds.Contains(e.ProjectId.Value)))
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = await BuildDtosAsync(db, events, cancellationToken);
        return Result<IReadOnlyList<ActivityEventDto>>.Success(dtos);
    }

    internal static async Task<List<ActivityEventDto>> BuildDtosAsync(AppDbContext db, List<ActivityEvent> events, CancellationToken ct)
    {
        var actorIds = events.Select(e => e.ActorUserId).Distinct().ToList();
        var actors = await db.Users.Where(u => actorIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, ct);

        var projectIds = events.Where(e => e.ProjectId != null).Select(e => e.ProjectId!.Value).Distinct().ToList();
        var projects = await db.Projects.Where(p => projectIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, ct);

        return events.Select(e =>
        {
            var actorUsername = actors.TryGetValue(e.ActorUserId, out var actor) ? actor.Username : "unknown";
            var project = e.ProjectId != null && projects.TryGetValue(e.ProjectId.Value, out var p) ? p : null;

            return new ActivityEventDto(
                e.Id,
                e.Type,
                e.ActorUserId,
                actorUsername,
                project?.Id,
                project?.Slug,
                project?.Title,
                ActivityEventSummaryBuilder.Build(e.Type, e.Payload, actorUsername, project?.Title),
                e.CreatedAt);
        }).ToList();
    }
}
