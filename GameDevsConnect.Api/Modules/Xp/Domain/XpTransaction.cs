namespace GameDevsConnect.Api.Modules.Xp.Domain;

public enum XpReason
{
    QuestAccepted,
    DifficultyBonus,
}

// XP is only ever derived from the sum of these rows - never a direct
// User.Xp += amount write - so every point can be traced back to why a user
// has it (see README §24).
public class XpTransaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Amount { get; set; }
    public XpReason Reason { get; set; }
    public required string SourceType { get; set; }
    public Guid SourceId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
