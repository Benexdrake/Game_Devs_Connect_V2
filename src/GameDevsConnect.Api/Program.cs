using GameDevsConnect.Api.Infrastructure.Persistence;
using GameDevsConnect.Api.Modules.Auth.Endpoints;
using GameDevsConnect.Api.Modules.Auth.GitHub;
using GameDevsConnect.Api.Modules.Projects.Endpoints;
using GameDevsConnect.Api.Modules.Skills.Endpoints;
using GameDevsConnect.Api.Modules.Users.Endpoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.Configure<GitHubOAuthOptions>(builder.Configuration.GetSection(GitHubOAuthOptions.SectionName));
builder.Services.AddHttpClient<GitHubOAuthClient>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Whether cookies require HTTPS. True behind the Caddy/TLS setup on the VPS,
// false for plain-HTTP local runs (dotnet run, or docker compose without the
// "proxy" profile) - deliberately independent of ASPNETCORE_ENVIRONMENT, since
// docker compose runs the api in Production mode even without TLS in front.
var requireHttps = builder.Configuration.GetValue("Cookies:RequireHttps", true);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "gdc_session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = requireHttps
            ? CookieSecurePolicy.Always
            : CookieSecurePolicy.None;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapSkillEndpoints();
app.MapProjectEndpoints();

app.Run();
