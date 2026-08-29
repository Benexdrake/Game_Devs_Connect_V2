using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Notifications.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Notifications.Queries;

public record NotificationDto(Guid Id, NotificationType Type, string Message, bool IsRead, DateTimeOffset CreatedAt);

public record NotificationsResultDto(IReadOnlyList<NotificationDto> Items, int UnreadCount);

public record GetNotificationsQuery(Guid RequestingUserId, int Page, int PageSize) : IRequest<Result<NotificationsResultDto>>;

public class GetNotificationsQueryHandler(AppDbContext db)
    : IRequestHandler<GetNotificationsQuery, Result<NotificationsResultDto>>
{
    public async Task<Result<NotificationsResultDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var notifications = await db.Notifications
            .Where(n => n.UserId == request.RequestingUserId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto(n.Id, n.Type, n.Message, n.IsRead, n.CreatedAt))
            .ToListAsync(cancellationToken);

        var unreadCount = await db.Notifications.CountAsync(
            n => n.UserId == request.RequestingUserId && !n.IsRead, cancellationToken);

        return Result<NotificationsResultDto>.Success(new NotificationsResultDto(notifications, unreadCount));
    }
}
