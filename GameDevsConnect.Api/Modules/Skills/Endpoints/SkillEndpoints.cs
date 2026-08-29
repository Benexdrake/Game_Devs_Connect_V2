using GameDevsConnect.Api.Modules.Skills.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using MediatR;

namespace GameDevsConnect.Api.Modules.Skills.Endpoints;

public static class SkillEndpoints
{
    public static void MapSkillEndpoints(this WebApplication app)
    {
        app.MapGet("/api/skills", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetSkillsQuery(), ct);
            return result.ToHttpResult();
        });
    }
}
