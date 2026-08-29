using System.Text.RegularExpressions;
using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects.Commands;

public record CreateProjectCommand(
    Guid CreatorUserId,
    string Title,
    string? Description,
    string? BannerUrl,
    Guid? EngineId,
    IReadOnlyList<Guid>? GenreIds,
    string? GitHubRepoFullName,
    ProjectStatus Status,
    ProjectVisibility Visibility) : IRequest<Result<ProjectDto>>;

public partial class CreateProjectCommandHandler(AppDbContext db)
    : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<ProjectDto>.ValidationError("Title is required.");
        }

        if (request.EngineId is not null && !await db.Engines.AnyAsync(e => e.Id == request.EngineId, cancellationToken))
        {
            return Result<ProjectDto>.ValidationError("Unknown engine.");
        }

        var slug = await MakeUniqueSlugAsync(Slugify(request.Title), cancellationToken);
        var now = DateTimeOffset.UtcNow;

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Slug = slug,
            Title = request.Title,
            Description = request.Description,
            BannerUrl = request.BannerUrl,
            EngineId = request.EngineId,
            GitHubRepoFullName = request.GitHubRepoFullName,
            Status = request.Status,
            Visibility = request.Visibility,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.Projects.Add(project);

        db.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = project.Id,
            UserId = request.CreatorUserId,
            Role = ProjectRole.Owner,
            JoinedAt = now,
        });

        if (request.GenreIds is { Count: > 0 })
        {
            var validGenreIds = await db.Genres
                .Where(g => request.GenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync(cancellationToken);
            foreach (var genreId in validGenreIds)
            {
                db.ProjectGenres.Add(new ProjectGenre { ProjectId = project.Id, GenreId = genreId });
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result<ProjectDto>.Success(await ProjectDtoBuilder.BuildAsync(db, project, cancellationToken));
    }

    private async Task<string> MakeUniqueSlugAsync(string desired, CancellationToken ct)
    {
        var candidate = desired;
        var suffix = 1;
        while (await db.Projects.AnyAsync(p => p.Slug == candidate, ct))
        {
            candidate = $"{desired}-{suffix++}";
        }

        return candidate;
    }

    private static string Slugify(string title)
    {
        var lowered = title.Trim().ToLowerInvariant();
        var slug = NonSlugCharacters().Replace(lowered, "-").Trim('-');
        return slug.Length == 0 ? "project" : slug;
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonSlugCharacters();
}
