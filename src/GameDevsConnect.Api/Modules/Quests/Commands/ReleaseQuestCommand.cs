using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests.Commands;

public record ReleaseQuestCommand(Guid QuestId, Guid RequestingUserId) : IRequest<Result<QuestDto>>;

public class ReleaseQuestCommandHandler(AppDbContext db) : IRequestHandler<ReleaseQuestCommand, Result<QuestDto>>
{
    public async Task<Result<QuestDto>> Handle(ReleaseQuestCommand request, CancellationToken cancellationToken)
    {
        var quest = await db.Quests.FirstOrDefaultAsync(q => q.Id == request.QuestId, cancellationToken);
        if (quest is null)
        {
            return Result<QuestDto>.NotFound("Quest not found.");
        }

        var assignment = await db.QuestAssignments.FirstOrDefaultAsync(
            a => a.QuestId == quest.Id && a.UserId == request.RequestingUserId && a.ReleasedAt == null,
            cancellationToken);
        if (assignment is null)
        {
            return Result<QuestDto>.Conflict("You have not claimed this quest.");
        }

        var now = DateTimeOffset.UtcNow;
        assignment.ReleasedAt = now;

        var remainingActive = await db.QuestAssignments
            .CountAsync(a => a.QuestId == quest.Id && a.ReleasedAt == null, cancellationToken);
        if (remainingActive == 0)
        {
            quest.Status = QuestStatus.Open;
        }
        quest.UpdatedAt = now;

        await db.SaveChangesAsync(cancellationToken);

        return Result<QuestDto>.Success(await QuestDtoBuilder.BuildAsync(db, quest, cancellationToken));
    }
}
