namespace GameDevsConnect.Api.Modules.Projects.Domain;

public enum ProjectStatus
{
    Concept,
    InDevelopment,
    Beta,
    Released,
    Archived,
}

public enum ProjectVisibility
{
    Public,
    Private,
}

public class Project
{
    public Guid Id { get; set; }
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public Guid? EngineId { get; set; }
    public string? GitHubRepoFullName { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Concept;
    public ProjectVisibility Visibility { get; set; } = ProjectVisibility.Public;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
