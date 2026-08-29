using GameDevsConnect.Api.Modules.Engines.Domain;
using GameDevsConnect.Api.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace GameDevsConnect.Api.Modules.Projects.Data;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Slug).IsRequired();
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.Property(p => p.Title).IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().IsRequired();
        builder.Property(p => p.Visibility).HasConversion<string>().IsRequired();

        builder.HasOne<Engine>().WithMany().HasForeignKey(p => p.EngineId).OnDelete(DeleteBehavior.SetNull);

        builder.Property<NpgsqlTsVector>("SearchVector")
            .IsGeneratedTsVectorColumn("english", [nameof(Project.Title), nameof(Project.Description)]);
        builder.HasIndex("SearchVector").HasMethod("GIN");
    }
}
