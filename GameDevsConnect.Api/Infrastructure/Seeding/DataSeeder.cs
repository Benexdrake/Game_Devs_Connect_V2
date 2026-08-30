using System.Text.Json;
using System.Text.Json.Serialization;
using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Engines.Domain;
using GameDevsConnect.Api.Modules.Genres.Domain;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Infrastructure.Seeding;

// Loads dummy Users/Projects/Quests from seed-data.json for local development.
// Toggled via the "Seed:Enabled" config flag (see appsettings.Development.json)
// and only runs once - a GitHub-ID prefix marks rows as seed data so re-runs
// on an already-seeded database are a no-op instead of duplicating rows.
public static class DataSeeder
{
    private const string SeedFileName = "Infrastructure/Seeding/seed-data.json";
    private const string SeedGitHubIdPrefix = "seed-";

    public static async Task SeedAsync(AppDbContext db, ILogger logger)
    {
        if (await db.Users.AnyAsync(u => u.GitHubId.StartsWith(SeedGitHubIdPrefix)))
        {
            logger.LogInformation("Seed data already present, skipping.");
            return;
        }

        var path = Path.Combine(AppContext.BaseDirectory, SeedFileName);
        if (!File.Exists(path))
        {
            logger.LogWarning("Seed data file not found at {Path}, skipping seeding.", path);
            return;
        }

        var json = await File.ReadAllTextAsync(path);
        var data = JsonSerializer.Deserialize<SeedRoot>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        }) ?? throw new InvalidOperationException("Seed data file is empty or invalid.");

        var now = DateTimeOffset.UtcNow;

        // Skills/Genres/Engines are shared reference data that may already contain
        // rows (created by real users, or a prior seed run) - reuse existing rows
        // by name instead of blind-inserting, or this collides with their unique
        // name indexes.
        var skillsByName = (await db.Skills.ToListAsync()).ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);
        var genresByName = (await db.Genres.ToListAsync()).ToDictionary(g => g.Name, StringComparer.OrdinalIgnoreCase);
        var enginesByName = (await db.Engines.ToListAsync()).ToDictionary(e => e.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var seedSkill in data.Skills)
        {
            if (!skillsByName.ContainsKey(seedSkill.Name))
            {
                var skill = new Skill { Id = Guid.NewGuid(), Name = seedSkill.Name, Category = seedSkill.Category };
                db.Skills.Add(skill);
                skillsByName[seedSkill.Name] = skill;
            }
        }

        foreach (var name in data.Genres)
        {
            if (!genresByName.ContainsKey(name))
            {
                var genre = new Genre { Id = Guid.NewGuid(), Name = name };
                db.Genres.Add(genre);
                genresByName[name] = genre;
            }
        }

        foreach (var name in data.Engines)
        {
            if (!enginesByName.ContainsKey(name))
            {
                var engine = new Engine { Id = Guid.NewGuid(), Name = name };
                db.Engines.Add(engine);
                enginesByName[name] = engine;
            }
        }

        var userIndex = 0;
        foreach (var seedUser in data.Users)
        {
            var userCreatedAt = now.AddDays(-90 + userIndex * 3);
            var user = new User
            {
                Id = Guid.NewGuid(),
                GitHubId = $"{SeedGitHubIdPrefix}{seedUser.GitHubId}",
                Username = seedUser.Username,
                AvatarUrl = seedUser.AvatarUrl,
                Bio = seedUser.Bio,
                CreatedAt = userCreatedAt,
                UpdatedAt = userCreatedAt,
            };
            db.Users.Add(user);

            foreach (var skillName in seedUser.Skills)
            {
                if (skillsByName.TryGetValue(skillName, out var skill))
                {
                    db.UserSkills.Add(new UserSkill { UserId = user.Id, SkillId = skill.Id });
                }
            }

            var projectIndex = 0;
            foreach (var seedProject in seedUser.Projects)
            {
                var projectCreatedAt = userCreatedAt.AddDays(5 + projectIndex * 7);
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Slug = seedProject.Slug,
                    Title = seedProject.Title,
                    Description = seedProject.Description,
                    LogoUrl = seedProject.LogoUrl,
                    BannerUrl = seedProject.BannerUrl,
                    EngineId = seedProject.Engine is not null && enginesByName.TryGetValue(seedProject.Engine, out var engine)
                        ? engine.Id
                        : null,
                    Status = seedProject.Status,
                    Visibility = seedProject.Visibility,
                    CreatedAt = projectCreatedAt,
                    UpdatedAt = projectCreatedAt,
                };
                db.Projects.Add(project);
                db.ProjectMembers.Add(new ProjectMember
                {
                    ProjectId = project.Id,
                    UserId = user.Id,
                    Role = ProjectRole.Owner,
                    JoinedAt = projectCreatedAt,
                });

                foreach (var genreName in seedProject.Genres)
                {
                    if (genresByName.TryGetValue(genreName, out var genre))
                    {
                        db.ProjectGenres.Add(new ProjectGenre { ProjectId = project.Id, GenreId = genre.Id });
                    }
                }

                var questIndex = 0;
                foreach (var seedQuest in seedProject.Quests)
                {
                    var questCreatedAt = projectCreatedAt.AddDays(2 + questIndex * 2);
                    var quest = new Quest
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = project.Id,
                        CreatorId = user.Id,
                        Title = seedQuest.Title,
                        Description = seedQuest.Description,
                        Category = seedQuest.Category,
                        Difficulty = seedQuest.Difficulty,
                        XpReward = QuestDifficultyXp.For(seedQuest.Difficulty),
                        Status = seedQuest.Status,
                        Deadline = seedQuest.DeadlineDaysFromNow is int days ? now.AddDays(days) : null,
                        MaxContributors = seedQuest.MaxContributors,
                        CreatedAt = questCreatedAt,
                        UpdatedAt = questCreatedAt,
                    };
                    db.Quests.Add(quest);

                    foreach (var skillName in seedQuest.Skills)
                    {
                        if (skillsByName.TryGetValue(skillName, out var skill))
                        {
                            db.QuestSkills.Add(new QuestSkill { QuestId = quest.Id, SkillId = skill.Id });
                        }
                    }

                    questIndex++;
                }

                projectIndex++;
            }

            userIndex++;
        }

        await db.SaveChangesAsync();

        logger.LogInformation(
            "Seeded {UserCount} users, {ProjectCount} projects, {QuestCount} quests.",
            data.Users.Count,
            data.Users.Sum(u => u.Projects.Count),
            data.Users.Sum(u => u.Projects.Sum(p => p.Quests.Count)));
    }

    private sealed class SeedRoot
    {
        public List<string> Genres { get; set; } = [];
        public List<string> Engines { get; set; } = [];
        public List<SeedSkill> Skills { get; set; } = [];
        public List<SeedUser> Users { get; set; } = [];
    }

    private sealed class SeedSkill
    {
        public required string Name { get; set; }
        public SkillCategory Category { get; set; }
    }

    private sealed class SeedUser
    {
        public required string GitHubId { get; set; }
        public required string Username { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public List<string> Skills { get; set; } = [];
        public List<SeedProject> Projects { get; set; } = [];
    }

    private sealed class SeedProject
    {
        public required string Slug { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? Engine { get; set; }
        public ProjectStatus Status { get; set; }
        public ProjectVisibility Visibility { get; set; }
        public List<string> Genres { get; set; } = [];
        public List<SeedQuest> Quests { get; set; } = [];
    }

    private sealed class SeedQuest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public SkillCategory Category { get; set; }
        public QuestDifficulty Difficulty { get; set; }
        public QuestStatus Status { get; set; }
        public int? DeadlineDaysFromNow { get; set; }
        public int MaxContributors { get; set; } = 1;
        public List<string> Skills { get; set; } = [];
    }
}
