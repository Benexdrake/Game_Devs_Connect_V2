using System.Security.Claims;
using GameDevsConnect.Api.Modules.Auth.Commands;
using GameDevsConnect.Api.Modules.Auth.Queries;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GameDevsConnect.Api.Modules.Auth.Endpoints;

public static class AuthEndpoints
{
    private const string OAuthStateCookie = "gdc_oauth_state";

    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapGet("/login/github", async (IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new LoginWithGitHubCommand(), ct);
            if (!result.IsSuccess)
            {
                return result.ToHttpResult();
            }

            http.Response.Cookies.Append(OAuthStateCookie, result.Value!.State, new CookieOptions
            {
                HttpOnly = true,
                Secure = app.Configuration.GetValue("Cookies:RequireHttps", true),
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromMinutes(10),
            });

            return Results.Redirect(result.Value.AuthorizeUrl);
        });

        group.MapGet("/callback/github", async (
            string code,
            string state,
            IMediator mediator,
            HttpContext http,
            IConfiguration config,
            CancellationToken ct) =>
        {
            var cookieState = http.Request.Cookies[OAuthStateCookie];
            http.Response.Cookies.Delete(OAuthStateCookie);

            var result = await mediator.Send(new GitHubCallbackCommand(code, state, cookieState), ct);
            if (!result.IsSuccess)
            {
                return result.ToHttpResult();
            }

            var user = result.Value!;
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            var frontendBaseUrl = config["Frontend:BaseUrl"] ?? "http://localhost:3000";
            return Results.Redirect(frontendBaseUrl);
        });

        group.MapGet("/me", async (IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCurrentUserQuery(http.GetUserId()), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        group.MapPost("/logout", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Ok();
        });
    }
}
