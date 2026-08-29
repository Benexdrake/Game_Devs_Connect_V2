using GameDevsConnect.Api.Modules.Contributions.Domain;
using GameDevsConnect.Api.Modules.Projects.Domain;
using GameDevsConnect.Api.Modules.Quests.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Contributions.Data;

public class ContributionConfiguration : IEntityTypeConfiguration<Contribution>
{
    public void Configure(EntityTypeBuilder<Contribution> builder)
    {
        builder.ToTable("contributions");

        builder.HasKey(c => c.Id);

        builder.HasOne<User>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Project>().WithMany().HasForeignKey(c => c.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Quest>().WithMany().HasForeignKey(c => c.QuestId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<QuestSubmission>().WithMany().HasForeignKey(c => c.SubmissionId).OnDelete(DeleteBehavior.Restrict);
    }
}
