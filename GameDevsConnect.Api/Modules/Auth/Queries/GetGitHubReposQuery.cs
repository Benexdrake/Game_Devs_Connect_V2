using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Auth.GitHub;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Auth.Queries;

public record GitHubRepoDto(string FullName, string Name, string? Description, bool Private, DateTimeOffset UpdatedAt);

public record GetGitHubReposQuery(Guid UserId) : IRequest<Result<IReadOnlyList<GitHubRepoDto>>>;

public class GetGitHubReposQueryHandler(AppDbContext db, GitHubOAuthClient gitHubClient)
    : IRequestHandler<GetGitHubReposQuery, Result<IReadOnlyList<GitHubRepoDto>>>
{
    public async Task<Result<IReadOnlyList<GitHubRepoDto>>> Handle(GetGitHubReposQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user?.GitHubAccessToken is null)
        {
            return Result<IReadOnlyList<GitHubRepoDto>>.Unauthorized("GitHub ist nicht verbunden. Bitte erneut einloggen.");
        }

        var repos = await gitHubClient.GetUserRepositoriesAsync(user.GitHubAccessToken, cancellationToken);
        if (repos is null)
        {
            return Result<IReadOnlyList<GitHubRepoDto>>.Unauthorized("GitHub-Zugriff wurde widerrufen. Bitte erneut einloggen.");
        }

        var dtos = repos
            .Select(r => new GitHubRepoDto(r.FullName, r.Name, r.Description, r.Private, r.UpdatedAt))
            .ToList();

        return Result<IReadOnlyList<GitHubRepoDto>>.Success(dtos);
    }
}
