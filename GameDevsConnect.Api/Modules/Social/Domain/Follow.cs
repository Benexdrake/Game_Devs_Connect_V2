namespace GameDevsConnect.Api.Modules.Social.Domain;

public enum FollowTargetType
{
    User,
    Project,
}

public class Follow
{
    public Guid Id { get; set; }
    public Guid FollowerUserId { get; set; }
    public FollowTargetType TargetType { get; set; }
    public Guid TargetId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
