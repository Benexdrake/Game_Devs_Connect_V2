namespace GameDevsConnect.Api.Shared.Storage;

public interface IFileStorage
{
    Task SaveAsync(string relativePath, Stream content, CancellationToken ct);

    Stream OpenRead(string relativePath);
}
