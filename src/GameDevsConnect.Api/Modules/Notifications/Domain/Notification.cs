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
    public DateTimeOffset CreatedAt { get; set; }
}
