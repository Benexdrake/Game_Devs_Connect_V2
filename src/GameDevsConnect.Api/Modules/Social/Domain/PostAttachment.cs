namespace GameDevsConnect.Api.Modules.Social.Domain;

public class PostAttachment
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public required string StoragePath { get; set; }
}
