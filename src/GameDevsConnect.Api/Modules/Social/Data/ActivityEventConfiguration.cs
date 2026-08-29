using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Social.Data;

public class ActivityEventConfiguration : IEntityTypeConfiguration<ActivityEvent>
{
    public void Configure(EntityTypeBuilder<ActivityEvent> builder)
    {
        builder.ToTable("activity_events");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Type).HasConversion<string>().IsRequired();
        builder.Property(e => e.Payload).HasColumnType("jsonb");
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.ProjectId);

        builder.HasOne<Project>().WithMany().HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(e => e.ActorUserId).OnDelete(DeleteBehavior.Cascade);
    }
}
