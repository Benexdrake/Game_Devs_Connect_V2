using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDevsConnect.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReadAtToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReadAt",
                table: "notifications",
                type: "timestamp with time zone",
                nullable: true);

            // Backfill so pre-migration read notifications aren't permanently
            // excluded from the 24h cleanup - CreatedAt is the closest known
            // approximation of when they were actually read.
            migrationBuilder.Sql(@"
                UPDATE notifications
                SET ""ReadAt"" = ""CreatedAt""
                WHERE ""IsRead"" = true AND ""ReadAt"" IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "notifications");
        }
    }
}
