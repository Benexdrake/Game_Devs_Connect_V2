using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Social.Data;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("follows");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.TargetType).HasConversion<string>().IsRequired();
        builder.HasIndex(f => new { f.FollowerUserId, f.TargetType, f.TargetId }).IsUnique();

        builder.HasOne<User>().WithMany().HasForeignKey(f => f.FollowerUserId).OnDelete(DeleteBehavior.Cascade);
    }
}
