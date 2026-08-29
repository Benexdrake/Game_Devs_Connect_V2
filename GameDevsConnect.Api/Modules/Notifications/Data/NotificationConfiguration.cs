using GameDevsConnect.Api.Modules.Notifications.Domain;
using GameDevsConnect.Api.Modules.Social.Domain;
using GameDevsConnect.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Notifications.Data;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Type).HasConversion<string>().IsRequired();
        builder.Property(n => n.Message).IsRequired();
        builder.HasIndex(n => new { n.UserId, n.CreatedAt });

        builder.HasOne<User>().WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ActivityEvent>().WithMany().HasForeignKey(n => n.ActivityEventId).OnDelete(DeleteBehavior.SetNull);
    }
}
