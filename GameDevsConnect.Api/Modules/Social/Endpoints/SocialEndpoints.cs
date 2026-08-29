using GameDevsConnect.Api.Modules.Social.Commands;
using GameDevsConnect.Api.Modules.Social.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using GameDevsConnect.Api.Shared.Storage;
using MediatR;

namespace GameDevsConnect.Api.Modules.Social.Endpoints;

public record CreateCommentRequest(string Body);

public static class SocialEndpoints
{
    public static void MapSocialEndpoints(this WebApplication app)
    {
        // Follow / unfollow
        app.MapPost("/api/users/{username}/follow", async (string username, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new FollowUserCommand(username, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapDelete("/api/users/{username}/follow", async (string username, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new UnfollowUserCommand(username, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapPost("/api/projects/{slug}/follow", async (string slug, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new FollowProjectCommand(slug, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapDelete("/api/projects/{slug}/follow", async (string slug, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new UnfollowProjectCommand(slug, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        // Feed / activity
        app.MapGet("/api/feed", async (int? page, int? pageSize, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetFeedQuery(http.GetUserId(), page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapGet("/api/projects/{slug}/activity", async (string slug, int? page, int? pageSize, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var userId = http.User.Identity?.IsAuthenticated == true ? http.GetUserId() : (Guid?)null;
            var result = await mediator.Send(new GetProjectActivityQuery(slug, userId, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        });

        // Posts
        var projectPosts = app.MapGroup("/api/projects/{slug}/posts");

        projectPosts.MapPost("/", async (string slug, HttpRequest request, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var form = await request.ReadFormAsync(ct);
            var body = form["body"].ToString();
            var attachments = form.Files
                .Select(f => new UploadedFileInput(f.FileName, f.ContentType, f.Length, f.OpenReadStream()))
                .ToList();

            var result = await mediator.Send(new CreatePostCommand(slug, http.GetUserId(), body, attachments), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        projectPosts.MapGet("/", async (string slug, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var userId = http.User.Identity?.IsAuthenticated == true ? http.GetUserId() : (Guid?)null;
            var result = await mediator.Send(new GetProjectPostsQuery(slug, userId), ct);
            return result.ToHttpResult();
        });

        app.MapDelete("/api/posts/{id:guid}", async (Guid id, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeletePostCommand(id, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapGet("/api/posts/{id:guid}/attachments/{attachmentId:guid}", async (Guid id, Guid attachmentId, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var userId = http.User.Identity?.IsAuthenticated == true ? http.GetUserId() : (Guid?)null;
            var result = await mediator.Send(new GetPostAttachmentQuery(id, attachmentId, userId), ct);
            if (!result.IsSuccess)
            {
                return result.ToHttpResult();
            }

            var file = result.Value!;
            return Results.File(file.Content, file.ContentType, file.FileName);
        });

        // Comments
        app.MapPost("/api/posts/{id:guid}/comments", async (Guid id, CreateCommentRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new CreateCommentCommand(id, http.GetUserId(), body.Body), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapDelete("/api/comments/{id:guid}", async (Guid id, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteCommentCommand(id, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        // Likes
        app.MapPost("/api/posts/{id:guid}/like", async (Guid id, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new LikePostCommand(id, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapDelete("/api/posts/{id:guid}/like", async (Guid id, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new UnlikePostCommand(id, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
