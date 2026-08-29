using Microsoft.Extensions.Options;

namespace GameDevsConnect.Api.Shared.Storage;

public class LocalFileStorage(IOptions<StorageOptions> options) : IFileStorage
{
    public async Task SaveAsync(string relativePath, Stream content, CancellationToken ct)
    {
        var fullPath = Path.Combine(options.Value.UploadsPath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, ct);
    }

    public Stream OpenRead(string relativePath) =>
        File.OpenRead(Path.Combine(options.Value.UploadsPath, relativePath));
}
