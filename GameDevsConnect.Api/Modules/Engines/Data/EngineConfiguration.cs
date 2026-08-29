using GameDevsConnect.Api.Modules.Engines.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Engines.Data;

public class EngineConfiguration : IEntityTypeConfiguration<Engine>
{
    public void Configure(EntityTypeBuilder<Engine> builder)
    {
        builder.ToTable("engines");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired();
        builder.HasIndex(e => e.Name).IsUnique();

        builder.HasData(
            Seed("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e01", "Unity"),
            Seed("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e02", "Unreal Engine"),
            Seed("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e03", "Godot"),
            Seed("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e04", "GameMaker"),
            Seed("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e05", "Construct"),
            Seed("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e06", "RPG Maker"),
            Seed("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e07", "Custom Engine"),
            Seed("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e08", "Other")
        );
    }

    private static Engine Seed(string id, string name) => new() { Id = Guid.Parse(id), Name = name };
}
