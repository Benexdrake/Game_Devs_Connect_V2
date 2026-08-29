namespace GameDevsConnect.Api.Modules.Notifications.Domain;

public enum NotificationType
{
    SubmissionReviewed,
    NewQuestInFollowedProject,
    NewFollower,
}

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public required string Message { get; set; }
    public Guid? ActivityEventId { get; set; }
    public bool IsRead { get; set; }
    /// <summary>Set the moment IsRead flips to true - the cleanup job deletes rows 24h past this.</summary>
    public DateTimeOffset? ReadAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
