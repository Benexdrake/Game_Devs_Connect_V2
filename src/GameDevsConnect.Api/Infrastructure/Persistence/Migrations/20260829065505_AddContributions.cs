using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDevsConnect.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContributions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quest_submissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewComment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quest_submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_quest_submissions_quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "quests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quest_submissions_users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_quest_submissions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contributions_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contributions_quest_submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "quest_submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contributions_quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "quests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contributions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "submission_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_submission_files_quest_submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "quest_submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "submission_links",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_submission_links_quest_submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "quest_submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contributions_ProjectId",
                table: "contributions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_contributions_QuestId",
                table: "contributions",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_contributions_SubmissionId",
                table: "contributions",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_contributions_UserId",
                table: "contributions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_quest_submissions_QuestId",
                table: "quest_submissions",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_quest_submissions_ReviewerId",
                table: "quest_submissions",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_quest_submissions_UserId",
                table: "quest_submissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_submission_files_SubmissionId",
                table: "submission_files",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_submission_links_SubmissionId",
                table: "submission_links",
                column: "SubmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contributions");

            migrationBuilder.DropTable(
                name: "submission_files");

            migrationBuilder.DropTable(
                name: "submission_links");

            migrationBuilder.DropTable(
                name: "quest_submissions");
        }
    }
}
