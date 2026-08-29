using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Skills.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Quests.Data;

public class QuestSkillConfiguration : IEntityTypeConfiguration<QuestSkill>
{
    public void Configure(EntityTypeBuilder<QuestSkill> builder)
    {
        builder.ToTable("quest_skills");

        builder.HasKey(qs => new { qs.QuestId, qs.SkillId });

        builder.HasOne<Quest>().WithMany().HasForeignKey(qs => qs.QuestId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Skill>().WithMany().HasForeignKey(qs => qs.SkillId).OnDelete(DeleteBehavior.Cascade);
    }
}
