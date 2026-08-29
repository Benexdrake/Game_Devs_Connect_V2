namespace GameDevsConnect.Api.Modules.Contributions.Domain;

public class SubmissionFile
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public required string StoragePath { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}
