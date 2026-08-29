using GameDevsConnect.Api.Modules.Projects.Commands;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Projects.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using MediatR;

namespace GameDevsConnect.Api.Modules.Projects.Endpoints;

public record CreateProjectRequest(string Title, string? Description, string? Engine, string? Genre, ProjectVisibility Visibility);

public record UpdateProjectRequest(
    string? Title,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    string? Engine,
    string? Genre,
    ProjectStatus? Status,
    ProjectVisibility? Visibility);

public record AddProjectMemberRequest(string Username, ProjectRole Role);

public record ChangeProjectMemberRoleRequest(ProjectRole Role);

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projects");

        group.MapPost("/", async (CreateProjectRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new CreateProjectCommand(http.GetUserId(), body.Title, body.Description, body.Engine, body.Genre, body.Visibility), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        group.MapGet("/{slug}", async (string slug, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var userId = http.User.Identity?.IsAuthenticated == true ? http.GetUserId() : (Guid?)null;
            var result = await mediator.Send(new GetProjectQuery(slug, userId), ct);
            return result.ToHttpResult();
        });

        group.MapPatch("/{slug}", async (string slug, UpdateProjectRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new UpdateProjectCommand(slug, http.GetUserId(), body.Title, body.Description, body.LogoUrl,
                    body.BannerUrl, body.Engine, body.Genre, body.Status, body.Visibility), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        group.MapDelete("/{slug}", async (string slug, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteProjectCommand(slug, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        group.MapPost("/{slug}/members", async (string slug, AddProjectMemberRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new AddProjectMemberCommand(slug, http.GetUserId(), body.Username, body.Role), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        group.MapPatch("/{slug}/members/{username}", async (string slug, string username, ChangeProjectMemberRoleRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new ChangeProjectMemberRoleCommand(slug, http.GetUserId(), username, body.Role), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        group.MapDelete("/{slug}/members/{username}", async (string slug, string username, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new RemoveProjectMemberCommand(slug, http.GetUserId(), username), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
