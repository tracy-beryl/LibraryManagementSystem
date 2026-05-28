using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddStudentProgress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentResourceProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(nullable: false),
                    ResourceId = table.Column<int>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    CompletedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentResourceProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentResourceProgresses_LibraryResources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "LibraryResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentResourceProgresses_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6e588ed-980a-4f74-a5fa-d36f468fbf24", new DateTime(2026, 2, 26, 10, 19, 49, 496, DateTimeKind.Local).AddTicks(8137), "AQAAAAEAACcQAAAAEDgL5oEZWn9bO1QRvZFwMUaKNg0ZX7QX/9yScoylaq+AhmTx7QYUAWqbXJEh+GnH+Q==", "7bc30eb9-0ca9-4839-9103-e6350818f2e3" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentResourceProgresses_ResourceId",
                table: "StudentResourceProgresses",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentResourceProgresses_StudentProfileId",
                table: "StudentResourceProgresses",
                column: "StudentProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentResourceProgresses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4d0cd2cb-eb87-449a-9dc9-983f89feceb9", new DateTime(2026, 2, 24, 18, 41, 42, 1, DateTimeKind.Local).AddTicks(3358), "AQAAAAEAACcQAAAAEIGUyrjSQOnp+dxJuYD/b/oAnI5fQpNPeZ7C/uJuqaM4ang+9/IdNCWSX+KctXOYng==", "7b18f77f-8786-4a75-b169-1f5d525febc9" });
        }
    }
}
