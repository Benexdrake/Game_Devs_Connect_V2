using GameDevsConnect.Api.Shared;
using GameDevsConnect.Api.Shared.Storage;
using MediatR;

namespace GameDevsConnect.Api.Modules.Uploads.Commands;

public record UploadImageResultDto(string Url);

public record UploadImageCommand(Guid UserId, UploadedFileInput File) : IRequest<Result<UploadImageResultDto>>;

public class UploadImageCommandHandler(IFileStorage fileStorage) : IRequestHandler<UploadImageCommand, Result<UploadImageResultDto>>
{
    private const long MaxSizeBytes = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> AllowedContentTypes = new()
    {
        ["image/png"] = "png",
        ["image/jpeg"] = "jpg",
        ["image/webp"] = "webp",
        ["image/gif"] = "gif",
    };

    public async Task<Result<UploadImageResultDto>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        var file = request.File;

        if (file.SizeBytes <= 0)
        {
            return Result<UploadImageResultDto>.ValidationError("File is empty.");
        }

        if (file.SizeBytes > MaxSizeBytes)
        {
            return Result<UploadImageResultDto>.ValidationError("File exceeds the 5 MB size limit.");
        }

        if (!AllowedContentTypes.TryGetValue(file.ContentType, out var extension))
        {
            return Result<UploadImageResultDto>.ValidationError("Unsupported file type. Allowed: PNG, JPEG, WEBP, GIF.");
        }

        var relativePath = Path.Combine("images", request.UserId.ToString(), $"{Guid.NewGuid()}.{extension}");
        await fileStorage.SaveAsync(relativePath, file.Content, cancellationToken);

        var url = "/uploads/" + relativePath.Replace(Path.DirectorySeparatorChar, '/');
        return Result<UploadImageResultDto>.Success(new UploadImageResultDto(url));
    }
}
