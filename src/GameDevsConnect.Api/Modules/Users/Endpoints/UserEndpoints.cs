using GameDevsConnect.Api.Modules.Users.Commands;
using GameDevsConnect.Api.Modules.Users.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using MediatR;

namespace GameDevsConnect.Api.Modules.Users.Endpoints;

public record UpdateUserProfileRequest(string? Bio, IReadOnlyList<UserLinkInput>? Links);

public record SetUserSkillsRequest(IReadOnlyList<Guid> SkillIds);

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/{username}", async (string username, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var userId = http.User.Identity?.IsAuthenticated == true ? http.GetUserId() : (Guid?)null;
            var result = await mediator.Send(new GetUserProfileQuery(username, userId), ct);
            return result.ToHttpResult();
        });

        group.MapPatch("/me", async (UpdateUserProfileRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new UpdateUserProfileCommand(http.GetUserId(), body.Bio, body.Links), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        group.MapPut("/me/skills", async (SetUserSkillsRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new SetUserSkillsCommand(http.GetUserId(), body.SkillIds), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
