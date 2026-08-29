namespace GameDevsConnect.Api.Modules.Social.Domain;

public class Post
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid AuthorId { get; set; }
    public required string Body { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
