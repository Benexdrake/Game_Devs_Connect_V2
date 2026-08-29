using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Users.Data;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.GitHubId).IsRequired();
        builder.HasIndex(u => u.GitHubId).IsUnique();

        builder.Property(u => u.Username).IsRequired();
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.AvatarUrl);
        builder.Property(u => u.CreatedAt).IsRequired();
    }
}
