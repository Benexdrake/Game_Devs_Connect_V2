namespace GameDevsConnect.Api.Modules.Users.Domain;

public class UserLink
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Label { get; set; }
    public required string Url { get; set; }
}
