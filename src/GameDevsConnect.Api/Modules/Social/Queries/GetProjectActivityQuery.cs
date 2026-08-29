using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Quests;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Social.Queries;

public record GetProjectActivityQuery(string ProjectSlug, Guid? RequestingUserId, int Page, int PageSize)
    : IRequest<Result<IReadOnlyList<ActivityEventDto>>>;

public class GetProjectActivityQueryHandler(AppDbContext db)
    : IRequestHandler<GetProjectActivityQuery, Result<IReadOnlyList<ActivityEventDto>>>
{
    public async Task<Result<IReadOnlyList<ActivityEventDto>>> Handle(GetProjectActivityQuery request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.ProjectSlug, cancellationToken);
        if (project is null)
        {
            return Result<IReadOnlyList<ActivityEventDto>>.NotFound("Project not found.");
        }

        if (!await QuestAccess.CanViewProjectQuestsAsync(db, project, request.RequestingUserId, cancellationToken))
        {
            return Result<IReadOnlyList<ActivityEventDto>>.NotFound("Project not found.");
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var events = await db.ActivityEvents
            .Where(e => e.ProjectId == project.Id)
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = await GetFeedQueryHandler.BuildDtosAsync(db, events, cancellationToken);
        return Result<IReadOnlyList<ActivityEventDto>>.Success(dtos);
    }
}
