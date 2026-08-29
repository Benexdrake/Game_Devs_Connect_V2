using System.Security.Cryptography;
using GameDevsConnect.Api.Modules.Auth.GitHub;
using GameDevsConnect.Api.Shared;
using MediatR;

namespace GameDevsConnect.Api.Modules.Auth.Commands;

public record GitHubLoginRedirect(string AuthorizeUrl, string State);

public record LoginWithGitHubCommand : IRequest<Result<GitHubLoginRedirect>>;

public class LoginWithGitHubCommandHandler(GitHubOAuthClient gitHubClient)
    : IRequestHandler<LoginWithGitHubCommand, Result<GitHubLoginRedirect>>
{
    public Task<Result<GitHubLoginRedirect>> Handle(LoginWithGitHubCommand request, CancellationToken cancellationToken)
    {
        var state = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        var authorizeUrl = gitHubClient.BuildAuthorizeUrl(state);
        return Task.FromResult(Result<GitHubLoginRedirect>.Success(new GitHubLoginRedirect(authorizeUrl, state)));
    }
}
