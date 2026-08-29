using GameDevsConnect.Api.Modules.Social.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Social.Data;

public class PostAttachmentConfiguration : IEntityTypeConfiguration<PostAttachment>
{
    public void Configure(EntityTypeBuilder<PostAttachment> builder)
    {
        builder.ToTable("post_attachments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.FileName).IsRequired();
        builder.Property(a => a.ContentType).IsRequired();
        builder.Property(a => a.StoragePath).IsRequired();

        builder.HasOne<Post>().WithMany().HasForeignKey(a => a.PostId).OnDelete(DeleteBehavior.Cascade);
    }
}
