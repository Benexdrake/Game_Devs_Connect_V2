using GameDevsConnect.Api.Modules.Quests.Commands;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Quests.Queries;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using MediatR;

namespace GameDevsConnect.Api.Modules.Quests.Endpoints;

public record CreateQuestRequest(
    string Title,
    string? Description,
    SkillCategory Category,
    QuestDifficulty Difficulty,
    int XpReward,
    DateTimeOffset? Deadline,
    int? MaxContributors,
    IReadOnlyList<Guid>? RequiredSkillIds);

public record UpdateQuestRequest(
    string? Title,
    string? Description,
    SkillCategory? Category,
    QuestDifficulty? Difficulty,
    int? XpReward,
    DateTimeOffset? Deadline,
    int? MaxContributors,
    IReadOnlyList<Guid>? RequiredSkillIds);

public record QuestListQueryParams(
    string? Search,
    SkillCategory? Category,
    Guid? SkillId,
    string? ProjectSlug,
    QuestDifficulty? Difficulty,
    int? MinXp,
    string? Engine);

public static class QuestEndpoints
{
    public static void MapQuestEndpoints(this WebApplication app)
    {
        var projectQuests = app.MapGroup("/api/projects/{slug}/quests");

        projectQuests.MapPost("/", async (string slug, CreateQuestRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new CreateQuestCommand(slug, http.GetUserId(), body.Title, body.Description, body.Category,
                    body.Difficulty, body.XpReward, body.Deadline, body.MaxContributors, body.RequiredSkillIds), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        projectQuests.MapGet("/", async (string slug, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var userId = http.User.Identity?.IsAuthenticated == true ? http.GetUserId() : (Guid?)null;
            var result = await mediator.Send(new GetProjectQuestsQuery(slug, userId), ct);
            return result.ToHttpResult();
        });

        projectQuests.MapPatch("/{questId:guid}", async (string slug, Guid questId, UpdateQuestRequest body, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new UpdateQuestCommand(slug, questId, http.GetUserId(), body.Title, body.Description, body.Category,
                    body.Difficulty, body.XpReward, body.Deadline, body.MaxContributors, body.RequiredSkillIds), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        projectQuests.MapDelete("/{questId:guid}", async (string slug, Guid questId, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteQuestCommand(slug, questId, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        var quests = app.MapGroup("/api/quests");

        quests.MapGet("/", async ([AsParameters] QuestListQueryParams q, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var userId = http.User.Identity?.IsAuthenticated == true ? http.GetUserId() : (Guid?)null;
            var result = await mediator.Send(
                new GetQuestsQuery(q.Search, q.Category, q.SkillId, q.ProjectSlug, q.Difficulty, q.MinXp, q.Engine, userId), ct);
            return result.ToHttpResult();
        });

        quests.MapGet("/{questId:guid}", async (Guid questId, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var userId = http.User.Identity?.IsAuthenticated == true ? http.GetUserId() : (Guid?)null;
            var result = await mediator.Send(new GetQuestQuery(questId, userId), ct);
            return result.ToHttpResult();
        });

        quests.MapPost("/{questId:guid}/claim", async (Guid questId, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new ClaimQuestCommand(questId, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        quests.MapPost("/{questId:guid}/release", async (Guid questId, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new ReleaseQuestCommand(questId, http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
