using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Social.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Projects;

public record ProjectMemberDto(Guid UserId, string Username, string? AvatarUrl, ProjectRole Role);

public record ProjectDto(
    Guid Id,
    string Slug,
    string Title,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    string? Engine,
    string? Genre,
    ProjectStatus Status,
    ProjectVisibility Visibility,
    IReadOnlyList<string> Tags,
    IReadOnlyList<ProjectMemberDto> Members,
    bool IsFollowedByMe,
    DateTimeOffset CreatedAt);

internal static class ProjectDtoBuilder
{
    public static async Task<ProjectDto> BuildAsync(AppDbContext db, Project project, CancellationToken ct, Guid? requestingUserId = null)
    {
        var tags = await db.ProjectTags
            .Where(pt => pt.ProjectId == project.Id)
            .Join(db.Tags, pt => pt.TagId, t => t.Id, (pt, t) => t.Name)
            .ToListAsync(ct);

        var members = await db.ProjectMembers
            .Where(m => m.ProjectId == project.Id)
            .Join(db.Users, m => m.UserId, u => u.Id, (m, u) => new ProjectMemberDto(u.Id, u.Username, u.AvatarUrl, m.Role))
            .ToListAsync(ct);

        var isFollowedByMe = requestingUserId is not null && await db.Follows.AnyAsync(
            f => f.FollowerUserId == requestingUserId && f.TargetType == FollowTargetType.Project && f.TargetId == project.Id, ct);

        return new ProjectDto(
            project.Id,
            project.Slug,
            project.Title,
            project.Description,
            project.LogoUrl,
            project.BannerUrl,
            project.Engine,
            project.Genre,
            project.Status,
            project.Visibility,
            tags,
            members,
            isFollowedByMe,
            project.CreatedAt);
    }
}
