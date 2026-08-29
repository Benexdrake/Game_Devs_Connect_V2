using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Quests.Data;

public class QuestConfiguration : IEntityTypeConfiguration<Quest>
{
    public void Configure(EntityTypeBuilder<Quest> builder)
    {
        builder.ToTable("quests");

        builder.HasKey(q => q.Id);
        builder.Property(q => q.Title).IsRequired();
        builder.Property(q => q.Category).HasConversion<string>().IsRequired();
        builder.Property(q => q.Difficulty).HasConversion<string>().IsRequired();
        builder.Property(q => q.Status).HasConversion<string>().IsRequired();

        builder.HasOne<Project>().WithMany().HasForeignKey(q => q.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(q => q.CreatorId).OnDelete(DeleteBehavior.Cascade);
    }
}
