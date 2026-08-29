using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Quests.Commands;

public record ClaimQuestCommand(Guid QuestId, Guid RequestingUserId) : IRequest<Result<QuestDto>>;

public class ClaimQuestCommandHandler(AppDbContext db) : IRequestHandler<ClaimQuestCommand, Result<QuestDto>>
{
    public async Task<Result<QuestDto>> Handle(ClaimQuestCommand request, CancellationToken cancellationToken)
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

        if (quest.CreatorId == request.RequestingUserId)
        {
            return Result<QuestDto>.Forbidden("You cannot claim a quest you created.");
        }

        if (quest.Status != QuestStatus.Open)
        {
            return Result<QuestDto>.Conflict("Quest is not open for claiming.");
        }

        var activeClaims = await db.QuestAssignments
            .CountAsync(a => a.QuestId == quest.Id && a.ReleasedAt == null, cancellationToken);
        if (activeClaims >= quest.MaxContributors)
        {
            return Result<QuestDto>.Conflict("Quest is already fully claimed.");
        }

        var now = DateTimeOffset.UtcNow;
        db.QuestAssignments.Add(new QuestAssignment
        {
            Id = Guid.NewGuid(),
            QuestId = quest.Id,
            UserId = request.RequestingUserId,
            ClaimedAt = now,
        });
        quest.Status = QuestStatus.InProgress;
        quest.UpdatedAt = now;

        await db.SaveChangesAsync(cancellationToken);

        return Result<QuestDto>.Success(await QuestDtoBuilder.BuildAsync(db, quest, cancellationToken));
    }
}
