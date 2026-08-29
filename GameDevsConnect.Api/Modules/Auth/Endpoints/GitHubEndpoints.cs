using GameDevsConnect.Api.Modules.Auth.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using MediatR;

namespace GameDevsConnect.Api.Modules.Auth.Endpoints;

public static class GitHubEndpoints
{
    public static void MapGitHubEndpoints(this WebApplication app)
    {
        app.MapGet("/api/github/repos", async (IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetGitHubReposQuery(http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
