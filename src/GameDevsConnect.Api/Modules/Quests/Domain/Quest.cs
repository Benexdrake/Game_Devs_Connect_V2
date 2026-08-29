using GameDevsConnect.Api.Modules.Skills.Domain;

namespace GameDevsConnect.Api.Modules.Quests.Domain;

public enum QuestDifficulty
{
    Easy,
    Medium,
    Hard,
}

public enum QuestStatus
{
    Open,
    Claimed,
    InProgress,
    Submitted,
    InReview,
    ChangesRequested,
    Accepted,
    Rejected,
    Cancelled,
}

public class Quest
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid CreatorId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public SkillCategory Category { get; set; }
    public QuestDifficulty Difficulty { get; set; }
    public int XpReward { get; set; }
    public QuestStatus Status { get; set; } = QuestStatus.Open;
    public DateTimeOffset? Deadline { get; set; }
    public int MaxContributors { get; set; } = 1;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
