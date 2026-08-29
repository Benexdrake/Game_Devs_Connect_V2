using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace GameDevsConnect.Api.Modules.Auth.GitHub;

public record GitHubTokenResponse(
    [property: JsonPropertyName("access_token")] string? AccessToken,
    [property: JsonPropertyName("error")] string? Error);

public record GitHubUser(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("avatar_url")] string? AvatarUrl);

public record GitHubRepo(
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("private")] bool Private,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt);

public class GitHubOAuthClient(HttpClient httpClient, IOptions<GitHubOAuthOptions> options)
{
    private readonly GitHubOAuthOptions _options = options.Value;

    public string BuildAuthorizeUrl(string state)
    {
        var query = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["redirect_uri"] = _options.CallbackUrl,
            // No repo scope requested on purpose: GitHub grants read-only access to a
            // user's public repositories (listing, commits, issues, releases) to any
            // authenticated token regardless of scope. Adding "public_repo" would only
            // grant *write* access on top of that, which this app never needs.
            ["scope"] = "read:user",
            ["state"] = state,
            ["allow_signup"] = "true",
        };
        return QueryHelpers.AddQueryString("https://github.com/login/oauth/authorize", query);
    }

    public async Task<string?> ExchangeCodeForAccessTokenAsync(string code, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["code"] = code,
                ["redirect_uri"] = _options.CallbackUrl,
            }),
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var token = await response.Content.ReadFromJsonAsync<GitHubTokenResponse>(cancellationToken: ct);
        return token?.AccessToken;
    }

    public async Task<GitHubUser?> GetUserAsync(string accessToken, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.UserAgent.ParseAdd("GameDevsConnect");

        using var response = await httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<GitHubUser>(cancellationToken: ct);
    }

    public async Task<IReadOnlyList<GitHubRepo>?> GetUserRepositoriesAsync(string accessToken, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            "https://api.github.com/user/repos?affiliation=owner&sort=updated&per_page=100");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.UserAgent.ParseAdd("GameDevsConnect");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        using var response = await httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<List<GitHubRepo>>(cancellationToken: ct);
    }
}
