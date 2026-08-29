namespace GameDevsConnect.Api.Modules.Contributions.Domain;

public enum SubmissionStatus
{
    PendingReview,
    ChangesRequested,
    Accepted,
    Rejected,
}

public class QuestSubmission
{
    public Guid Id { get; set; }
    public Guid QuestId { get; set; }
    public Guid UserId { get; set; }
    public required string Description { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.PendingReview;
    public DateTimeOffset SubmittedAt { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public Guid? ReviewerId { get; set; }
    public string? ReviewComment { get; set; }
}
