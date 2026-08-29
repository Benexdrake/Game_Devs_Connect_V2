namespace GameDevsConnect.Api.Modules.Projects.Domain;

public enum ProjectRole
{
    Owner,
    Admin,
    Contributor,
}

public class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public ProjectRole Role { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
}
