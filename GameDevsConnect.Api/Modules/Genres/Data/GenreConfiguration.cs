using GameDevsConnect.Api.Modules.Genres.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Genres.Data;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("genres");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).IsRequired();
        builder.HasIndex(g => g.Name).IsUnique();

        builder.HasData(
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e01", "Action"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e02", "Adventure"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e03", "RPG"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e04", "Platformer"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e05", "Shooter"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e06", "Puzzle"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e07", "Simulation"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e08", "Strategy"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e09", "Horror"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e0a", "Roguelike"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e0b", "Sandbox"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e0c", "Visual Novel"),
            Seed("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e0d", "Other")
        );
    }

    private static Genre Seed(string id, string name) => new() { Id = Guid.Parse(id), Name = name };
}
