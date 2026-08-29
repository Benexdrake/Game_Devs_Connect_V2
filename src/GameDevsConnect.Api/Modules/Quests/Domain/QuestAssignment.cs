namespace GameDevsConnect.Api.Modules.Quests.Domain;

public class QuestAssignment
{
    public Guid Id { get; set; }
    public Guid QuestId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset ClaimedAt { get; set; }
    public DateTimeOffset? ReleasedAt { get; set; }
}
