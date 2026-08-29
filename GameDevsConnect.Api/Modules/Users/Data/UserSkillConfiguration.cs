using GameDevsConnect.Api.Modules.Skills.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Users.Data;

public class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
{
    public void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        builder.ToTable("user_skills");

        builder.HasKey(us => new { us.UserId, us.SkillId });

        builder.HasOne<User>().WithMany().HasForeignKey(us => us.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Skill>().WithMany().HasForeignKey(us => us.SkillId).OnDelete(DeleteBehavior.Cascade);
    }
}
