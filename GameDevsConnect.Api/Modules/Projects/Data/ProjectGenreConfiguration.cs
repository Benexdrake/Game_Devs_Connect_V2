using GameDevsConnect.Api.Modules.Genres.Domain;
using GameDevsConnect.Api.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Projects.Data;

public class ProjectGenreConfiguration : IEntityTypeConfiguration<ProjectGenre>
{
    public void Configure(EntityTypeBuilder<ProjectGenre> builder)
    {
        builder.ToTable("project_genres");

        builder.HasKey(pg => new { pg.ProjectId, pg.GenreId });

        builder.HasOne<Project>().WithMany().HasForeignKey(pg => pg.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Genre>().WithMany().HasForeignKey(pg => pg.GenreId).OnDelete(DeleteBehavior.Cascade);
    }
}
