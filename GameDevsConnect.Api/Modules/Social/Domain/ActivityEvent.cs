namespace GameDevsConnect.Api.Modules.Social.Domain;

public enum ActivityEventType
{
    QuestCreated,
    ContributionAccepted,
    MemberJoined,
    ProjectPosted,
    LevelUp,
}

public class ActivityEvent
{
    public Guid Id { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid ActorUserId { get; set; }
    public ActivityEventType Type { get; set; }
    public string? Payload { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
