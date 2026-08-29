using GameDevsConnect.Api.Modules.Uploads.Commands;
using GameDevsConnect.Api.Shared.Endpoints;
using GameDevsConnect.Api.Shared.Http;
using GameDevsConnect.Api.Shared.Storage;
using MediatR;

namespace GameDevsConnect.Api.Modules.Uploads.Endpoints;

public static class UploadEndpoints
{
    public static void MapUploadEndpoints(this WebApplication app)
    {
        app.MapPost("/api/uploads/images", async (IFormFile file, IMediator mediator, HttpContext http, CancellationToken ct) =>
        {
            var input = new UploadedFileInput(file.FileName, file.ContentType, file.Length, file.OpenReadStream());
            var result = await mediator.Send(new UploadImageCommand(http.GetUserId(), input), ct);
            return result.ToHttpResult();
        }).RequireAuthorization().DisableAntiforgery();
    }
}
