using GameDevsConnect.Api.Modules.Contributions.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Contributions.Data;

public class QuestSubmissionConfiguration : IEntityTypeConfiguration<QuestSubmission>
{
    public void Configure(EntityTypeBuilder<QuestSubmission> builder)
    {
        builder.ToTable("quest_submissions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Description).IsRequired();
        builder.Property(s => s.Status).HasConversion<string>().IsRequired();

        builder.HasOne<Quest>().WithMany().HasForeignKey(s => s.QuestId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(s => s.ReviewerId).OnDelete(DeleteBehavior.SetNull);
    }
}
