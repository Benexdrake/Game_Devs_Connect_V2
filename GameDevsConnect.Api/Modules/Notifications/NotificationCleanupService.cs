using GameDevsConnect.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Notifications;

/// <summary>Deletes notifications 24h after they were read - hourly is plenty granular for a day-long retention window.</summary>
public class NotificationCleanupService(IServiceScopeFactory scopeFactory, ILogger<NotificationCleanupService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);
    private static readonly TimeSpan RetentionAfterRead = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var cutoff = DateTimeOffset.UtcNow - RetentionAfterRead;

                var deleted = await db.Notifications
                    .Where(n => n.IsRead && n.ReadAt != null && n.ReadAt < cutoff)
                    .ExecuteDeleteAsync(stoppingToken);

                if (deleted > 0)
                {
                    logger.LogInformation("Deleted {Count} read notifications older than 24h.", deleted);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Notification cleanup pass failed.");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
