using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Notifications.Commands;

public record DeleteNotificationCommand(Guid NotificationId, Guid RequestingUserId) : IRequest<Result<bool>>;

public class DeleteNotificationCommandHandler(AppDbContext db) : IRequestHandler<DeleteNotificationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(
            n => n.Id == request.NotificationId && n.UserId == request.RequestingUserId, cancellationToken);
        if (notification is null)
        {
            return Result<bool>.NotFound("Notification not found.");
        }

        db.Notifications.Remove(notification);
        await db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
