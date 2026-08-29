using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Social.Data;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Body).IsRequired();

        builder.HasOne<Post>().WithMany().HasForeignKey(c => c.PostId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.AuthorId).OnDelete(DeleteBehavior.Cascade);
    }
}
