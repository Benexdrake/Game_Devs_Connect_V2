using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Social.Events;
using GameDevsConnect.Api.Modules.Xp.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameDevsConnect.Api.Modules.Xp.Commands;

// Awards XP for an accepted contribution. Amount = quest.XpReward + a
// difficulty bonus (Easy +0 / Medium +25 / Hard +75, see README §35 & Phase
// 4 spec). Capped by the rolling 24h daily limit - accepting a submission
// must never fail because of the cap, so we clamp the amount instead of
// rejecting the request.
public record AwardXpCommand(Guid UserId, QuestDifficulty Difficulty, int BaseAmount, string SourceType, Guid SourceId)
    : IRequest<Result<int>>;

public class AwardXpCommandHandler(AppDbContext db, IOptions<XpOptions> options, IMediator mediator)
    : IRequestHandler<AwardXpCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AwardXpCommand request, CancellationToken cancellationToken)
    {
        var difficultyBonus = request.Difficulty switch
        {
            QuestDifficulty.Easy => 0,
            QuestDifficulty.Medium => 25,
            QuestDifficulty.Hard => 75,
            _ => 0,
        };
        var desiredAmount = request.BaseAmount + difficultyBonus;

        var since = DateTimeOffset.UtcNow.AddHours(-24);
        var xpLast24h = await db.XpTransactions
            .Where(t => t.UserId == request.UserId && t.CreatedAt >= since)
            .SumAsync(t => (int?)t.Amount, cancellationToken) ?? 0;

        var remaining = Math.Max(0, options.Value.DailyCap - xpLast24h);
        var grantedAmount = Math.Min(desiredAmount, remaining);

        if (grantedAmount > 0)
        {
            var totalXpBefore = await db.XpTransactions
                .Where(t => t.UserId == request.UserId)
                .SumAsync(t => (int?)t.Amount, cancellationToken) ?? 0;

            db.XpTransactions.Add(new XpTransaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Amount = grantedAmount,
                Reason = XpReason.QuestAccepted,
                SourceType = request.SourceType,
                SourceId = request.SourceId,
                CreatedAt = DateTimeOffset.UtcNow,
            });

            await db.SaveChangesAsync(cancellationToken);

            var levelBefore = LevelCalculator.LevelForXp(totalXpBefore);
            var levelAfter = LevelCalculator.LevelForXp(totalXpBefore + grantedAmount);
            if (levelAfter > levelBefore)
            {
                await mediator.Publish(new UserLeveledUpEvent(request.UserId, levelAfter), cancellationToken);
            }
        }

        return Result<int>.Success(grantedAmount);
    }
}
