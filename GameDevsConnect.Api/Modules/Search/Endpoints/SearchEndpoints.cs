using GameDevsConnect.Api.Modules.Search.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using MediatR;

namespace GameDevsConnect.Api.Modules.Search.Endpoints;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this WebApplication app)
    {
        app.MapGet("/api/search", async (string q, string? type, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var userId = http.User.Identity?.IsAuthenticated == true ? http.GetUserId() : (Guid?)null;
            var result = await mediator.Send(new GetSearchQuery(q, type, userId), ct);
            return result.ToHttpResult();
        });
    }
}
