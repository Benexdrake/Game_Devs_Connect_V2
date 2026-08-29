using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Quests.Data;

public class QuestAssignmentConfiguration : IEntityTypeConfiguration<QuestAssignment>
{
    public void Configure(EntityTypeBuilder<QuestAssignment> builder)
    {
        builder.ToTable("quest_assignments");

        builder.HasKey(a => a.Id);

        builder.HasOne<Quest>().WithMany().HasForeignKey(a => a.QuestId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
