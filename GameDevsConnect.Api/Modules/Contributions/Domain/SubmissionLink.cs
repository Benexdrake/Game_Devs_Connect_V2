namespace GameDevsConnect.Api.Modules.Contributions.Domain;

public class SubmissionLink
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public required string Url { get; set; }
    public string? Label { get; set; }
}
