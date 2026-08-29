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

    private static string? GetString(JsonElement? root, string property) =>
        root?.TryGetProperty(property, out var value) == true ? value.GetString() : null;

    private static int? GetInt(JsonElement? root, string property) =>
        root?.TryGetProperty(property, out var value) == true ? value.GetInt32() : null;
}
