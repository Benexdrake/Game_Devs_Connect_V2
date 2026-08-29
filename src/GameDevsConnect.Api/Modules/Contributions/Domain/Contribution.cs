namespace GameDevsConnect.Api.Modules.Contributions.Domain;

public class Contribution
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid QuestId { get; set; }
    public Guid SubmissionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
