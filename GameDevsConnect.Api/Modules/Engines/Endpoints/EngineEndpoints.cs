using GameDevsConnect.Api.Modules.Engines.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using MediatR;

namespace GameDevsConnect.Api.Modules.Engines.Endpoints;

public static class EngineEndpoints
{
    public static void MapEngineEndpoints(this WebApplication app)
    {
        app.MapGet("/api/engines", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetEnginesQuery(), ct);
            return result.ToHttpResult();
        });
    }
}
