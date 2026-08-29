using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserLink> UserLinks => Set<UserLink>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ProjectTag> ProjectTags => Set<ProjectTag>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<QuestSkill> QuestSkills => Set<QuestSkill>();
    public DbSet<QuestAssignment> QuestAssignments => Set<QuestAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
