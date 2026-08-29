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
    /// <summary>Set for QuestCreated/ContributionAccepted - lets "for you" matching join straight to the quest's SkillCategory without parsing <see cref="Payload"/>.</summary>
    public Guid? QuestId { get; set; }
    public Guid ActorUserId { get; set; }
    public ActivityEventType Type { get; set; }
    public string? Payload { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
