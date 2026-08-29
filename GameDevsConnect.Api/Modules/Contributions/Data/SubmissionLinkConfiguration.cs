using GameDevsConnect.Api.Modules.Contributions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Contributions.Data;

public class SubmissionLinkConfiguration : IEntityTypeConfiguration<SubmissionLink>
{
    public void Configure(EntityTypeBuilder<SubmissionLink> builder)
    {
        builder.ToTable("submission_links");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Url).IsRequired();

        builder.HasOne<QuestSubmission>().WithMany().HasForeignKey(l => l.SubmissionId).OnDelete(DeleteBehavior.Cascade);
    }
}
