using GameDevsConnect.Api.Modules.Notifications.Commands;
using GameDevsConnect.Api.Modules.Notifications.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using MediatR;

namespace GameDevsConnect.Api.Modules.Notifications.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notifications").RequireAuthorization();

        group.MapGet("/", async (int? page, int? pageSize, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetNotificationsQuery(http.GetUserId(), page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        });

        group.MapPatch("/{id:guid}/read", async (Guid id, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new MarkNotificationReadCommand(id, http.GetUserId()), ct);
            return result.ToHttpResult();
        });

        group.MapPatch("/read-all", async (IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new MarkAllNotificationsReadCommand(http.GetUserId()), ct);
            return result.ToHttpResult();
        });
    }
}
