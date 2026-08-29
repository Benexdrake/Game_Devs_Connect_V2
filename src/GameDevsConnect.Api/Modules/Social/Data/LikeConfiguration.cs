using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Social.Data;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.ToTable("likes");

        builder.HasKey(l => new { l.UserId, l.PostId });

        builder.HasOne<User>().WithMany().HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Post>().WithMany().HasForeignKey(l => l.PostId).OnDelete(DeleteBehavior.Cascade);
    }
}
