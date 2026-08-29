namespace GameDevsConnect.Api.Modules.Users.Domain;

public class User
{
    public Guid Id { get; set; }
    public required string GitHubId { get; set; }
    public required string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
