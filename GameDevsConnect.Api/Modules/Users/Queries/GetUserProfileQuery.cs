using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Users.Queries;

public record UserLinkDto(string Label, string Url);

public record UserSkillDto(Guid Id, string Name, SkillCategory Category);

public record UserProjectSummaryDto(string Slug, string Title, string? LogoUrl, ProjectStatus Status);

public record UserContributionDto(
    Guid Id,
    string ProjectSlug,
    string ProjectTitle,
    Guid QuestId,
    string QuestTitle,
    DateTimeOffset CreatedAt);

public record UserProfileDto(
    Guid Id,
    string Username,
    string? AvatarUrl,
    string? Bio,
    IReadOnlyList<UserLinkDto> Links,
    IReadOnlyList<UserSkillDto> Skills,
    IReadOnlyList<UserProjectSummaryDto> Projects,
    IReadOnlyList<UserContributionDto> Contributions,
    bool IsFollowedByMe);

public record GetUserProfileQuery(string Username, Guid? RequestingUserId) : IRequest<Result<UserProfileDto>>;

public class GetUserProfileQueryHandler(AppDbContext db)
    : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        if (user is null)
        {
            return Result<UserProfileDto>.NotFound("User not found.");
        }

        var links = await db.UserLinks
            .Where(l => l.UserId == user.Id)
            .Select(l => new UserLinkDto(l.Label, l.Url))
            .ToListAsync(cancellationToken);

        var skills = await db.UserSkills
            .Where(us => us.UserId == user.Id)
            .Join(db.Skills, us => us.SkillId, s => s.Id, (us, s) => new UserSkillDto(s.Id, s.Name, s.Category))
            .ToListAsync(cancellationToken);

        var isOwnProfile = request.RequestingUserId == user.Id;
        var projects = await db.ProjectMembers
            .Where(m => m.UserId == user.Id)
            .Join(db.Projects, m => m.ProjectId, p => p.Id, (m, p) => p)
            .Where(p => isOwnProfile || p.Visibility == ProjectVisibility.Public)
            .Select(p => new UserProjectSummaryDto(p.Slug, p.Title, p.LogoUrl, p.Status))
            .ToListAsync(cancellationToken);

        var contributions = await db.Contributions
            .Where(c => c.UserId == user.Id)
            .OrderByDescending(c => c.CreatedAt)
            .Join(db.Projects, c => c.ProjectId, p => p.Id, (c, p) => new { c, p })
            .Join(db.Quests, x => x.c.QuestId, q => q.Id,
                (x, q) => new UserContributionDto(x.c.Id, x.p.Slug, x.p.Title, q.Id, q.Title, x.c.CreatedAt))
            .ToListAsync(cancellationToken);

        var isFollowedByMe = request.RequestingUserId is not null && await db.Follows.AnyAsync(
            f => f.FollowerUserId == request.RequestingUserId && f.TargetType == FollowTargetType.User && f.TargetId == user.Id,
            cancellationToken);

        return Result<UserProfileDto>.Success(
            new UserProfileDto(user.Id, user.Username, user.AvatarUrl, user.Bio, links, skills, projects, contributions, isFollowedByMe));
    }
}
