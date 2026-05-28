using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddLecturerUnitAssignments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LecturerUnitAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LecturerUserId = table.Column<string>(nullable: false),
                    Program = table.Column<string>(nullable: false),
                    Level = table.Column<string>(nullable: false),
                    Semester = table.Column<int>(nullable: false),
                    UnitCode = table.Column<string>(nullable: false),
                    UnitName = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LecturerUnitAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LecturerUnitAssignments_AspNetUsers_LecturerUserId",
                        column: x => x.LecturerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0811b73-5663-4668-a0a1-270a5c94ced3", new DateTime(2026, 3, 13, 21, 25, 33, 914, DateTimeKind.Local).AddTicks(6941), "AQAAAAEAACcQAAAAEN6jg8FmAaaiTqGlZRvyJfcd1MqeT3fZ2mgrTJPicWnU+7AQqWUDbtrcqwHZjeHcDw==", "3a59a637-16b4-4390-bb6c-3321cd6bb627" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturerUnitAssignments_LecturerUserId",
                table: "LecturerUnitAssignments",
                column: "LecturerUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LecturerUnitAssignments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "865162a5-c849-4218-aab3-3b0db8365ddd", new DateTime(2026, 3, 11, 0, 16, 45, 410, DateTimeKind.Local).AddTicks(9533), "AQAAAAEAACcQAAAAEJw4cnup5tJ9ceQxLfOBlgBLDNSWYg2YJXyjDV6TAzwPMAvN8eGLRWsKbsEJQAMDnA==", "3a0f0487-ffce-4a0e-aef0-4a039ba90b69" });
        }
    }
}
