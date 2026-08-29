using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDevsConnect.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestIdToActivityEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuestId",
                table: "activity_events",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_activity_events_QuestId",
                table: "activity_events",
                column: "QuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_activity_events_quests_QuestId",
                table: "activity_events",
                column: "QuestId",
                principalTable: "quests",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Backfill existing rows from their own jsonb payload so pre-migration
            // QuestCreated/ContributionAccepted events aren't silently excluded
            // from "for you" matching just because they predate this column.
            migrationBuilder.Sql(@"
                UPDATE activity_events
                SET ""QuestId"" = (""Payload""->>'questId')::uuid
                WHERE ""Type"" IN ('QuestCreated', 'ContributionAccepted')
                  AND ""QuestId"" IS NULL
                  AND ""Payload""->>'questId' IS NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_activity_events_quests_QuestId",
                table: "activity_events");

            migrationBuilder.DropIndex(
                name: "IX_activity_events_QuestId",
                table: "activity_events");

            migrationBuilder.DropColumn(
                name: "QuestId",
                table: "activity_events");
        }
    }
}
