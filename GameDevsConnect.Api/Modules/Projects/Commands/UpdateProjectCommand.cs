using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects.Commands;

public record UpdateProjectCommand(
    string Slug,
    Guid RequestingUserId,
    string? Title,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    Guid? EngineId,
    IReadOnlyList<Guid>? GenreIds,
    string? GitHubRepoFullName,
    ProjectStatus? Status,
    ProjectVisibility? Visibility) : IRequest<Result<ProjectDto>>;

public class UpdateProjectCommandHandler(AppDbContext db)
    : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);
        if (project is null)
        {
            return Result<ProjectDto>.NotFound("Project not found.");
        }

        var role = await ProjectAccess.GetRoleAsync(db, project.Id, request.RequestingUserId, cancellationToken);
        if (role is not (ProjectRole.Owner or ProjectRole.Admin))
        {
            return Result<ProjectDto>.Forbidden("Only the owner or an admin can edit this project.");
        }

        if (request.EngineId is not null && !await db.Engines.AnyAsync(e => e.Id == request.EngineId, cancellationToken))
        {
            return Result<ProjectDto>.ValidationError("Unknown engine.");
        }

        if (request.Title is not null) project.Title = request.Title;
        if (request.Description is not null) project.Description = request.Description;
        if (request.LogoUrl is not null) project.LogoUrl = request.LogoUrl;
        if (request.BannerUrl is not null) project.BannerUrl = request.BannerUrl;
        if (request.EngineId is not null) project.EngineId = request.EngineId;
        if (request.GitHubRepoFullName is not null) project.GitHubRepoFullName = request.GitHubRepoFullName;
        if (request.Status is not null) project.Status = request.Status.Value;
        if (request.Visibility is not null) project.Visibility = request.Visibility.Value;
        project.UpdatedAt = DateTimeOffset.UtcNow;

        if (request.GenreIds is not null)
        {
            var existing = await db.ProjectGenres.Where(pg => pg.ProjectId == project.Id).ToListAsync(cancellationToken);
            db.ProjectGenres.RemoveRange(existing);

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
}
