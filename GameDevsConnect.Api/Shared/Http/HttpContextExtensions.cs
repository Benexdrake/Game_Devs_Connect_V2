using System.Security.Claims;

namespace GameDevsConnect.Api.Shared.Http;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext http) =>
        Guid.Parse(http.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
