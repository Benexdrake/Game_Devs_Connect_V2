using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects.Queries;

public record DiscoverProjectsQuery(string Sort, int Page, int PageSize) : IRequest<Result<IReadOnlyList<ProjectDto>>>;

public class DiscoverProjectsQueryHandler(AppDbContext db) : IRequestHandler<DiscoverProjectsQuery, Result<IReadOnlyList<ProjectDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectDto>>> Handle(DiscoverProjectsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var publicProjects = db.Projects.Where(p => p.Visibility == ProjectVisibility.Public);

        List<Project> projects;
        switch (request.Sort)
        {
            case "trending":
                var since = DateTimeOffset.UtcNow.AddDays(-7);
                projects = await publicProjects
                    .Select(p => new
                    {
                        Project = p,
                        RecentActivity = db.ActivityEvents.Count(e => e.ProjectId == p.Id && e.CreatedAt >= since),
                    })
                    .OrderByDescending(x => x.RecentActivity)
                    .ThenByDescending(x => x.Project.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => x.Project)
                    .ToListAsync(cancellationToken);
                break;

            case "looking-for-contributors":
                projects = await publicProjects
                    .Where(p => db.Quests.Any(q => q.ProjectId == p.Id && q.Status == QuestStatus.Open))
                    .OrderByDescending(p => p.UpdatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);
                break;

            case "new":
                projects = await publicProjects
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);
                break;

            case "recent":
            default:
                projects = await publicProjects
                    .OrderByDescending(p => p.UpdatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);
                break;
        }

        var dtos = new List<ProjectDto>(projects.Count);
        foreach (var project in projects)
        {
            dtos.Add(await ProjectDtoBuilder.BuildAsync(db, project, cancellationToken));
        }

        return Result<IReadOnlyList<ProjectDto>>.Success(dtos);
    }
}
