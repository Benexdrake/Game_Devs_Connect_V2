using GameDevsConnect.Api.Modules.Users.Domain;
using GameDevsConnect.Api.Modules.Xp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Xp.Data;

public class XpTransactionConfiguration : IEntityTypeConfiguration<XpTransaction>
{
    public void Configure(EntityTypeBuilder<XpTransaction> builder)
    {
        builder.ToTable("xp_transactions");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Reason).HasConversion<string>().IsRequired();
        builder.Property(t => t.SourceType).IsRequired();
        builder.HasIndex(t => new { t.UserId, t.CreatedAt });

        builder.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
