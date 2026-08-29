using GameDevsConnect.Api.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Projects.Data;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired();
        builder.HasIndex(t => t.Name).IsUnique();
    }
}

public class ProjectTagConfiguration : IEntityTypeConfiguration<ProjectTag>
{
    public void Configure(EntityTypeBuilder<ProjectTag> builder)
    {
        builder.ToTable("project_tags");

        builder.HasKey(pt => new { pt.ProjectId, pt.TagId });

        builder.HasOne<Project>().WithMany().HasForeignKey(pt => pt.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Tag>().WithMany().HasForeignKey(pt => pt.TagId).OnDelete(DeleteBehavior.Cascade);
    }
}
