using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameDevsConnect.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestructureSkillCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("439780f2-c1da-4f58-9207-f6efe2badfd7"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("67aabe52-5d35-4e68-96da-ab51702eece9"));

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("2838e506-cb9c-408a-ac3e-2a525c3d982b"),
                column: "Category",
                value: "Engines");

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("30e3205f-3abd-4b5d-89e9-9d4b32f282b6"),
                column: "Category",
                value: "Engines");

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("b14f822f-009e-4bdd-8ee8-11f717c507e8"),
                column: "Category",
                value: "Engines");

            migrationBuilder.InsertData(
                table: "skills",
                columns: new[] { "Id", "Category", "Name" },
                values: new object[,]
                {
                    { new Guid("11128a9b-c4de-45b1-96de-f8c59c5abe33"), "Programming", "Lua" },
                    { new Guid("17174a66-afe0-4a05-904b-657273b638b2"), "Art3D", "Substance Painter" },
                    { new Guid("1cf30b71-1812-4221-be32-0f526a027bb3"), "Audio", "Voice Acting" },
                    { new Guid("275ac4a5-1897-4959-98be-a8a53c7e8372"), "Writing", "Localization" },
                    { new Guid("381cf829-8c1c-4808-876c-231bbdb13ba5"), "Production", "QA / Testing" },
                    { new Guid("447e1a2b-9b9c-44cf-afcc-7c2cfe5ae031"), "Production", "Community Management" },
                    { new Guid("4509c131-7068-4984-b592-78a6ac8a1163"), "Production", "Marketing" },
                    { new Guid("4688962e-b2f6-4958-9859-6c00d6783259"), "Art2D", "Illustrator" },
                    { new Guid("4f44b26c-63c8-4bf4-a6a5-69c070b7bc29"), "Programming", "GDScript" },
                    { new Guid("5eb0e9c4-dbad-426b-a429-c74aac945a24"), "Writing", "Dialogue Writing" },
                    { new Guid("76e0dfb8-b0d1-4151-b9ff-4e200ec03914"), "Engines", "GameMaker" },
                    { new Guid("78b25059-9ae8-408f-925c-75d98676263d"), "Art3D", "ZBrush" },
                    { new Guid("993fe2fc-8afc-40fe-9a83-e686ead6ccf3"), "Animation", "VFX" },
                    { new Guid("9b5c497b-1d79-4850-8123-8ed4525640e9"), "Design", "UI/UX Design" },
                    { new Guid("aa33b59d-e508-4576-b586-b7810ab89084"), "Art2D", "Aseprite" },
                    { new Guid("bb2e2d42-8ce1-4d6b-9457-9809aec7d707"), "Production", "Project Management" },
                    { new Guid("bf950d08-5ca0-431a-aaea-df42d829fcf1"), "Design", "Narrative Design" },
                    { new Guid("c72ce11d-8e03-4f87-9ee9-ddf2d7e80d3b"), "Programming", "Python" },
                    { new Guid("edbbeb85-0210-4a75-a354-8873ebee6f79"), "Audio", "Wwise / FMOD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("11128a9b-c4de-45b1-96de-f8c59c5abe33"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("17174a66-afe0-4a05-904b-657273b638b2"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("1cf30b71-1812-4221-be32-0f526a027bb3"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("275ac4a5-1897-4959-98be-a8a53c7e8372"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("381cf829-8c1c-4808-876c-231bbdb13ba5"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("447e1a2b-9b9c-44cf-afcc-7c2cfe5ae031"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("4509c131-7068-4984-b592-78a6ac8a1163"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("4688962e-b2f6-4958-9859-6c00d6783259"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("4f44b26c-63c8-4bf4-a6a5-69c070b7bc29"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("5eb0e9c4-dbad-426b-a429-c74aac945a24"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("76e0dfb8-b0d1-4151-b9ff-4e200ec03914"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("78b25059-9ae8-408f-925c-75d98676263d"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("993fe2fc-8afc-40fe-9a83-e686ead6ccf3"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("9b5c497b-1d79-4850-8123-8ed4525640e9"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("aa33b59d-e508-4576-b586-b7810ab89084"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("bb2e2d42-8ce1-4d6b-9457-9809aec7d707"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("bf950d08-5ca0-431a-aaea-df42d829fcf1"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("c72ce11d-8e03-4f87-9ee9-ddf2d7e80d3b"));

            migrationBuilder.DeleteData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("edbbeb85-0210-4a75-a354-8873ebee6f79"));

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("2838e506-cb9c-408a-ac3e-2a525c3d982b"),
                column: "Category",
                value: "Programming");

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("30e3205f-3abd-4b5d-89e9-9d4b32f282b6"),
                column: "Category",
                value: "Programming");

            migrationBuilder.UpdateData(
                table: "skills",
                keyColumn: "Id",
                keyValue: new Guid("b14f822f-009e-4bdd-8ee8-11f717c507e8"),
                column: "Category",
                value: "Programming");

            migrationBuilder.InsertData(
                table: "skills",
                columns: new[] { "Id", "Category", "Name" },
                values: new object[,]
                {
                    { new Guid("439780f2-c1da-4f58-9207-f6efe2badfd7"), "Art3D", "3D Art" },
                    { new Guid("67aabe52-5d35-4e68-96da-ab51702eece9"), "Art2D", "2D Art" }
                });
        }
    }
}
