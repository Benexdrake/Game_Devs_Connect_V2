namespace GameDevsConnect.Api.Shared.Storage;

public class StorageOptions
{
    public const string SectionName = "Storage";

    public required string UploadsPath { get; set; }
}
