namespace GameDevsConnect.Api.Modules.Skills.Domain;

public enum SkillCategory
{
    Programming,
    Engines,
    Art2D,
    Art3D,
    Animation,
    Audio,
    Design,
    Writing,
    Production,
    Other,
}

public class Skill
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public SkillCategory Category { get; set; }
}
