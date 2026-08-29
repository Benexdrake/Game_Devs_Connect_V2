using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Contributions.Domain;
using GameDevsConnect.Api.Modules.Xp.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Xp.Queries;

public record XpSummaryDto(
    int TotalXp,
    int Level,
    int XpForCurrentLevel,
    int XpForNextLevel,
    double? Reputation,
    int CompletedQuests,
    int AcceptedContributions);

public record GetUserXpSummaryQuery(string Username) : IRequest<Result<XpSummaryDto>>;

public class GetUserXpSummaryQueryHandler(AppDbContext db) : IRequestHandler<GetUserXpSummaryQuery, Result<XpSummaryDto>>
{
    public async Task<Result<XpSummaryDto>> Handle(GetUserXpSummaryQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        if (user is null)
        {
            return Result<XpSummaryDto>.NotFound("User not found.");
        }

        var totalXp = await db.XpTransactions
            .Where(t => t.UserId == user.Id)
            .SumAsync(t => (int?)t.Amount, cancellationToken) ?? 0;

        var level = LevelCalculator.LevelForXp(totalXp);
        var xpForCurrentLevel = LevelCalculator.ThresholdForLevel(level);
        var xpForNextLevel = LevelCalculator.XpForNextLevel(totalXp);

        var accepted = await db.QuestSubmissions.CountAsync(
            s => s.UserId == user.Id && s.Status == SubmissionStatus.Accepted, cancellationToken);
        var rejected = await db.QuestSubmissions.CountAsync(
            s => s.UserId == user.Id && s.Status == SubmissionStatus.Rejected, cancellationToken);
        var totalReviewed = accepted + rejected;

        // Fewer than 3 reviewed submissions: don't show a value yet, so one
        // early rejection doesn't permanently brand a newcomer.
        double? reputation = totalReviewed >= 3 ? Math.Round(5.0 * accepted / totalReviewed, 1) : null;

        var completedQuests = await db.Contributions
            .Where(c => c.UserId == user.Id)
            .Select(c => c.QuestId)
            .Distinct()
            .CountAsync(cancellationToken);
        var acceptedContributions = await db.Contributions.CountAsync(c => c.UserId == user.Id, cancellationToken);

        return Result<XpSummaryDto>.Success(
            new XpSummaryDto(totalXp, level, xpForCurrentLevel, xpForNextLevel, reputation, completedQuests, acceptedContributions));
    }
}
