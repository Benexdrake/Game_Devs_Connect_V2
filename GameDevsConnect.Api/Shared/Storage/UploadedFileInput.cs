namespace GameDevsConnect.Api.Shared.Storage;

public record UploadedFileInput(string FileName, string ContentType, long SizeBytes, Stream Content);
