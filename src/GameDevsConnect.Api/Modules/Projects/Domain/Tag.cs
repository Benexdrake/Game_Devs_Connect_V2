namespace GameDevsConnect.Api.Modules.Projects.Domain;

public class Tag
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}

public class ProjectTag
{
    public Guid ProjectId { get; set; }
    public Guid TagId { get; set; }
}
