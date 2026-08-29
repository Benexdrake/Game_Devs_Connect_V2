using GameDevsConnect.Api.Modules.Genres.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using MediatR;

namespace GameDevsConnect.Api.Modules.Genres.Endpoints;

public static class GenreEndpoints
{
    public static void MapGenreEndpoints(this WebApplication app)
    {
        app.MapGet("/api/genres", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetGenresQuery(), ct);
            return result.ToHttpResult();
        });
    }
}
