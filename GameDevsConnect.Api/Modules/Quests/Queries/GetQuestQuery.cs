using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests.Queries;

public record GetQuestQuery(Guid QuestId, Guid? RequestingUserId) : IRequest<Result<QuestDto>>;

public class GetQuestQueryHandler(AppDbContext db) : IRequestHandler<GetQuestQuery, Result<QuestDto>>
{
    public async Task<Result<QuestDto>> Handle(GetQuestQuery request, CancellationToken cancellationToken)
    {
        var quest = await db.Quests.FirstOrDefaultAsync(q => q.Id == request.QuestId, cancellationToken);
        if (quest is null)
        {
            return Result<QuestDto>.NotFound("Quest not found.");
        }

        var project = await db.Projects.FirstAsync(p => p.Id == quest.ProjectId, cancellationToken);
        if (!await QuestAccess.CanViewProjectQuestsAsync(db, project, request.RequestingUserId, cancellationToken))
        {
            return Result<QuestDto>.NotFound("Quest not found.");
        }

        return Result<QuestDto>.Success(await QuestDtoBuilder.BuildAsync(db, quest, cancellationToken));
    }
}
