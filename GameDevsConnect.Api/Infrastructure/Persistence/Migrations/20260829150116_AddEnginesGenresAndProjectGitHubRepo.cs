using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameDevsConnect.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnginesGenresAndProjectGitHubRepo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Engine",
                table: "projects");

            migrationBuilder.RenameColumn(
                name: "Genre",
                table: "projects",
                newName: "GitHubRepoFullName");

            migrationBuilder.AddColumn<string>(
                name: "GitHubAccessToken",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EngineId",
                table: "projects",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "engines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "project_genres",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_genres", x => new { x.ProjectId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_project_genres_genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_genres_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "engines",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e01"), "Unity" },
                    { new Guid("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e02"), "Unreal Engine" },
                    { new Guid("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e03"), "Godot" },
                    { new Guid("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e04"), "GameMaker" },
                    { new Guid("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e05"), "Construct" },
                    { new Guid("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e06"), "RPG Maker" },
                    { new Guid("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e07"), "Custom Engine" },
                    { new Guid("f1a3f6b0-3b3f-4c1a-9f0e-1a2b3c4d5e08"), "Other" }
                });

            migrationBuilder.InsertData(
                table: "genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e01"), "Action" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e02"), "Adventure" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e03"), "RPG" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e04"), "Platformer" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e05"), "Shooter" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e06"), "Puzzle" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e07"), "Simulation" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e08"), "Strategy" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e09"), "Horror" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e0a"), "Roguelike" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e0b"), "Sandbox" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e0c"), "Visual Novel" },
                    { new Guid("a2b4c6d8-1e2f-4a5b-8c6d-1a2b3c4d5e0d"), "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_projects_EngineId",
                table: "projects",
                column: "EngineId");

            migrationBuilder.CreateIndex(
                name: "IX_engines_Name",
                table: "engines",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_genres_Name",
                table: "genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_genres_GenreId",
                table: "project_genres",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_engines_EngineId",
                table: "projects",
                column: "EngineId",
                principalTable: "engines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_projects_engines_EngineId",
                table: "projects");

            migrationBuilder.DropTable(
                name: "engines");

            migrationBuilder.DropTable(
                name: "project_genres");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropIndex(
                name: "IX_projects_EngineId",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "GitHubAccessToken",
                table: "users");

            migrationBuilder.DropColumn(
                name: "EngineId",
                table: "projects");

            migrationBuilder.RenameColumn(
                name: "GitHubRepoFullName",
                table: "projects",
                newName: "Genre");

            migrationBuilder.AddColumn<string>(
                name: "Engine",
                table: "projects",
                type: "text",
                nullable: true);
        }
    }
}
