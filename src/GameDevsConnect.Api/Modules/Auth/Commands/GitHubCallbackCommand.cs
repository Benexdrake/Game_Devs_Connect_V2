using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Auth.GitHub;
using GameDevsConnect.Api.Modules.Users.Domain;
using GameDevsConnect.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameDevsConnect.Api.Modules.Auth.Commands;

public record AuthenticatedUser(Guid Id, string Username, string? AvatarUrl);

public record GitHubCallbackCommand(string Code, string State, string? CookieState) : IRequest<Result<AuthenticatedUser>>;

public class GitHubCallbackCommandHandler(GitHubOAuthClient gitHubClient, AppDbContext db)
    : IRequestHandler<GitHubCallbackCommand, Result<AuthenticatedUser>>
{
    public async Task<Result<AuthenticatedUser>> Handle(GitHubCallbackCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.CookieState) || request.State != request.CookieState)
        {
            return Result<AuthenticatedUser>.Forbidden("Invalid OAuth state.");
        }

        var accessToken = await gitHubClient.ExchangeCodeForAccessTokenAsync(request.Code, cancellationToken);
        if (accessToken is null)
        {
            return Result<AuthenticatedUser>.Forbidden("Could not exchange GitHub authorization code.");
        }

        var gitHubUser = await gitHubClient.GetUserAsync(accessToken, cancellationToken);
        if (gitHubUser is null)
        {
            return Result<AuthenticatedUser>.Forbidden("Could not fetch GitHub profile.");
        }

        var gitHubId = gitHubUser.Id.ToString();
        var user = await db.Users.FirstOrDefaultAsync(u => u.GitHubId == gitHubId, cancellationToken);

        if (user is null)
        {
            var username = await MakeUniqueUsernameAsync(gitHubUser.Login, cancellationToken);
            var now = DateTimeOffset.UtcNow;
            user = new User
            {
                Id = Guid.NewGuid(),
                GitHubId = gitHubId,
                Username = username,
                AvatarUrl = gitHubUser.AvatarUrl,
                CreatedAt = now,
                UpdatedAt = now,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);
        }
        else if (user.AvatarUrl != gitHubUser.AvatarUrl)
        {
            user.AvatarUrl = gitHubUser.AvatarUrl;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }

        return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user.Id, user.Username, user.AvatarUrl));
    }

    private async Task<string> MakeUniqueUsernameAsync(string desired, CancellationToken cancellationToken)
    {
        var candidate = desired;
        var suffix = 1;
        while (await db.Users.AnyAsync(u => u.Username == candidate, cancellationToken))
        {
            candidate = $"{desired}-{suffix++}";
        }

        return candidate;
    }
}
