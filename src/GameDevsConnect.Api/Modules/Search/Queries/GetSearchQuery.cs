using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Search.Queries;

public record SearchProjectDto(string Slug, string Title, string? LogoUrl, string? Engine, string? Genre);

public record SearchQuestDto(Guid Id, string Title, string ProjectSlug, string ProjectTitle, QuestDifficulty Difficulty, int XpReward);

public record SearchUserDto(string Username, string? AvatarUrl);

public record SearchResultsDto(
    IReadOnlyList<SearchProjectDto> Projects,
    IReadOnlyList<SearchQuestDto> Quests,
    IReadOnlyList<SearchUserDto> Users);

public record GetSearchQuery(string Q, string? Type, Guid? RequestingUserId) : IRequest<Result<SearchResultsDto>>;

public class GetSearchQueryHandler(AppDbContext db) : IRequestHandler<GetSearchQuery, Result<SearchResultsDto>>
{
    private const int MaxResults = 20;

    public async Task<Result<SearchResultsDto>> Handle(GetSearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Q))
        {
            return Result<SearchResultsDto>.ValidationError("q is required.");
        }

        var q = request.Q;

        var projects = request.Type is null or "projects"
            ? await db.Projects
                .Where(p => p.Visibility == ProjectVisibility.Public)
                .Where(p => EF.Property<NpgsqlTypes.NpgsqlTsVector>(p, "SearchVector").Matches(EF.Functions.PlainToTsQuery("english", q)))
                .OrderByDescending(p => p.UpdatedAt)
                .Take(MaxResults)
                .Select(p => new SearchProjectDto(p.Slug, p.Title, p.LogoUrl, p.Engine, p.Genre))
                .ToListAsync(cancellationToken)
            : [];

        var quests = request.Type is null or "quests"
            ? await db.Quests
                .Join(db.Projects, quest => quest.ProjectId, p => p.Id, (quest, p) => new { Quest = quest, Project = p })
                .Where(x => x.Project.Visibility == ProjectVisibility.Public ||
                    (request.RequestingUserId != null &&
                        db.ProjectMembers.Any(m => m.ProjectId == x.Project.Id && m.UserId == request.RequestingUserId)))
                .Where(x => EF.Property<NpgsqlTypes.NpgsqlTsVector>(x.Quest, "SearchVector").Matches(EF.Functions.PlainToTsQuery("english", q)))
                .OrderByDescending(x => x.Quest.CreatedAt)
                .Take(MaxResults)
                .Select(x => new SearchQuestDto(x.Quest.Id, x.Quest.Title, x.Project.Slug, x.Project.Title, x.Quest.Difficulty, x.Quest.XpReward))
                .ToListAsync(cancellationToken)
            : [];

        var users = request.Type is null or "users"
            ? await db.Users
                .Where(u => EF.Functions.ILike(u.Username, $"%{request.Q}%"))
                .OrderBy(u => u.Username)
                .Take(MaxResults)
                .Select(u => new SearchUserDto(u.Username, u.AvatarUrl))
                .ToListAsync(cancellationToken)
            : [];

        return Result<SearchResultsDto>.Success(new SearchResultsDto(projects, quests, users));
    }
}
