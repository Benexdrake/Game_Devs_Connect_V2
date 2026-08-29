using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests.Queries;

public record GetProjectQuestsQuery(string ProjectSlug, Guid? RequestingUserId) : IRequest<Result<IReadOnlyList<QuestDto>>>;

public class GetProjectQuestsQueryHandler(AppDbContext db)
    : IRequestHandler<GetProjectQuestsQuery, Result<IReadOnlyList<QuestDto>>>
{
    public async Task<Result<IReadOnlyList<QuestDto>>> Handle(GetProjectQuestsQuery request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.ProjectSlug, cancellationToken);
        if (project is null)
        {
            return Result<IReadOnlyList<QuestDto>>.NotFound("Project not found.");
        }

        if (!await QuestAccess.CanViewProjectQuestsAsync(db, project, request.RequestingUserId, cancellationToken))
        {
            return Result<IReadOnlyList<QuestDto>>.NotFound("Project not found.");
        }

        var quests = await db.Quests
            .Where(q => q.ProjectId == project.Id)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = new List<QuestDto>(quests.Count);
        foreach (var quest in quests)
        {
            dtos.Add(await QuestDtoBuilder.BuildAsync(db, quest, cancellationToken));
        }

        return Result<IReadOnlyList<QuestDto>>.Success(dtos);
    }
}
