using GameDevsConnect.Api.Modules.Xp.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using MediatR;

namespace GameDevsConnect.Api.Modules.Xp.Endpoints;

public static class XpEndpoints
{
    public static void MapXpEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users/{username}/xp-summary", async (string username, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetUserXpSummaryQuery(username), ct);
            return result.ToHttpResult();
        });
    }
}
