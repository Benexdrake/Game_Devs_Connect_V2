using GameDevsConnect.Api.Modules.Contributions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Contributions.Data;

public class SubmissionFileConfiguration : IEntityTypeConfiguration<SubmissionFile>
{
    public void Configure(EntityTypeBuilder<SubmissionFile> builder)
    {
        builder.ToTable("submission_files");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.FileName).IsRequired();
        builder.Property(f => f.ContentType).IsRequired();
        builder.Property(f => f.StoragePath).IsRequired();

        builder.HasOne<QuestSubmission>().WithMany().HasForeignKey(f => f.SubmissionId).OnDelete(DeleteBehavior.Cascade);
    }
}
