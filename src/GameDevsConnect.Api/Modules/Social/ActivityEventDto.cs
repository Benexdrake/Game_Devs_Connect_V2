using System.Text.Json;
using GameDevsConnect.Api.Modules.Social.Domain;

namespace GameDevsConnect.Api.Modules.Social;

public record ActivityEventDto(
    Guid Id,
    ActivityEventType Type,
    Guid ActorUserId,
    string ActorUsername,
    Guid? ProjectId,
    string? ProjectSlug,
    string? ProjectTitle,
    string Summary,
    string? LinkUrl,
    DateTimeOffset CreatedAt);

internal static class ActivityEventSummaryBuilder
{
    public static string Build(ActivityEventType type, string? payloadJson, string actorUsername, string? projectTitle)
    {
        using var doc = string.IsNullOrEmpty(payloadJson) ? null : JsonDocument.Parse(payloadJson);
        var root = doc?.RootElement;

        return type switch
        {
            ActivityEventType.QuestCreated =>
                $"{actorUsername} created a new quest \"{GetString(root, "questTitle")}\" in {projectTitle}",
            ActivityEventType.ContributionAccepted =>
                $"{actorUsername}'s contribution was accepted for quest \"{GetString(root, "questTitle")}\" in {projectTitle}",
            ActivityEventType.MemberJoined =>
                $"{actorUsername} joined {projectTitle}",
            ActivityEventType.ProjectPosted =>
                $"{actorUsername} posted an update in {projectTitle}",
            ActivityEventType.LevelUp =>
                $"{actorUsername} reached level {GetInt(root, "newLevel")}",
            _ => $"{actorUsername} did something.",
        };
    }

    // Where clicking the feed/activity item should take you. Quest-related
    // events link straight to the quest, everything else falls back to the
    // project (or the actor's profile for a level-up, which has no project).
    public static string? BuildLink(ActivityEventType type, string? payloadJson, string actorUsername, string? projectSlug)
    {
        using var doc = string.IsNullOrEmpty(payloadJson) ? null : JsonDocument.Parse(payloadJson);
        var root = doc?.RootElement;

        return type switch
        {
            ActivityEventType.QuestCreated or ActivityEventType.ContributionAccepted =>
                GetString(root, "questId") is { } questId ? $"/quests/{questId}" : null,
            ActivityEventType.MemberJoined or ActivityEventType.ProjectPosted =>
                projectSlug is not null ? $"/projects/{projectSlug}" : null,
            ActivityEventType.LevelUp => $"/users/{actorUsername}",
            _ => null,
        };
    }

    private static string? GetString(JsonElement? root, string property) =>
        root?.TryGetProperty(property, out var value) == true ? value.GetString() : null;

    private static int? GetInt(JsonElement? root, string property) =>
        root?.TryGetProperty(property, out var value) == true ? value.GetInt32() : null;
}
