namespace GameDevsConnect.Api.Modules.Social.Domain;

public class Like
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
