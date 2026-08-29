using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Notifications.Commands;

public record MarkNotificationReadCommand(Guid NotificationId, Guid RequestingUserId) : IRequest<Result<bool>>;

public class MarkNotificationReadCommandHandler(AppDbContext db) : IRequestHandler<MarkNotificationReadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(
            n => n.Id == request.NotificationId && n.UserId == request.RequestingUserId, cancellationToken);
        if (notification is null)
        {
            return Result<bool>.NotFound("Notification not found.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result<bool>.Success(true);
    }
}

public record MarkAllNotificationsReadCommand(Guid RequestingUserId) : IRequest<Result<bool>>;

public class MarkAllNotificationsReadCommandHandler(AppDbContext db) : IRequestHandler<MarkAllNotificationsReadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        await db.Notifications
            .Where(n => n.UserId == request.RequestingUserId && !n.IsRead)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, now), cancellationToken);

        return Result<bool>.Success(true);
    }
}
