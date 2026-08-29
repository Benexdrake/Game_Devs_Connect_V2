using GameDevsConnect.Api.Modules.Contributions.Domain;
using GameDevsConnect.Api.Modules.Engines.Domain;
using GameDevsConnect.Api.Modules.Genres.Domain;
using GameDevsConnect.Api.Modules.Notifications.Domain;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using GameDevsConnect.Api.Modules.Xp.Domain;
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
    public DbSet<Engine> Engines => Set<Engine>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<ProjectGenre> ProjectGenres => Set<ProjectGenre>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<QuestSkill> QuestSkills => Set<QuestSkill>();
    public DbSet<QuestAssignment> QuestAssignments => Set<QuestAssignment>();
    public DbSet<QuestSubmission> QuestSubmissions => Set<QuestSubmission>();
    public DbSet<SubmissionFile> SubmissionFiles => Set<SubmissionFile>();
    public DbSet<SubmissionLink> SubmissionLinks => Set<SubmissionLink>();
    public DbSet<Contribution> Contributions => Set<Contribution>();
    public DbSet<XpTransaction> XpTransactions => Set<XpTransaction>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostAttachment> PostAttachments => Set<PostAttachment>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<ActivityEvent> ActivityEvents => Set<ActivityEvent>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
