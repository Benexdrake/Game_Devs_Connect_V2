using GameDevsConnect.Api.Modules.Contributions.Commands;
using GameDevsConnect.Api.Modules.Contributions.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using GameDevsConnect.Api.Shared.Storage;
using MediatR;

namespace GameDevsConnect.Api.Modules.Contributions.Endpoints;

public record CreateSubmissionRequest(string Description, IReadOnlyList<SubmissionLinkInput>? Links);

public record ReviewSubmissionRequest(SubmissionDecision Decision, string? Comment);

public static class ContributionEndpoints
{
    public static void MapContributionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/quests/{questId:guid}/submissions", async (Guid questId, CreateSubmissionRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new CreateSubmissionCommand(questId, http.GetUserId(), body.Description, body.Links), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapGet("/api/quests/{questId:guid}/submissions", async (Guid questId, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetQuestSubmissionsQuery(questId, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapPost("/api/submissions/{id:guid}/files", async (Guid id, IFormFileCollection files, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var inputs = files.Select(f => new UploadedFileInput(f.FileName, f.ContentType, f.Length, f.OpenReadStream())).ToList();
            var result = await mediator.Send(new UploadSubmissionFilesCommand(id, http.GetUserId(), inputs), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        app.MapGet("/api/submissions/{id:guid}/files/{fileId:guid}", async (Guid id, Guid fileId, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetSubmissionFileQuery(id, fileId, http.GetUserId()), ct);
            if (!result.IsSuccess)
            {
                return result.ToHttpResult();
            }

            var file = result.Value!;
            return Results.File(file.Content, file.ContentType, file.FileName);
        }).RequireAuthorization();

        app.MapPost("/api/submissions/{id:guid}/review", async (Guid id, ReviewSubmissionRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new ReviewSubmissionCommand(id, http.GetUserId(), body.Decision, body.Comment), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
