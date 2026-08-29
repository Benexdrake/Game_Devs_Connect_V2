namespace GameDevsConnect.Api.Modules.Users.Domain;

public enum LinkPlatform
{
    X,
    GitHub,
    LinkedIn,
    Instagram,
    YouTube,
    Twitch,
    Discord,
    TikTok,
    ItchIo,
    Reddit,
    Bluesky,
    Other,
}

public class UserLink
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public LinkPlatform Platform { get; set; }
    /// <summary>Only meaningful (and set) when <see cref="Platform"/> is <see cref="LinkPlatform.Other"/> - named platforms derive their display name from the platform itself.</summary>
    public string? Label { get; set; }
    public required string Url { get; set; }
}
