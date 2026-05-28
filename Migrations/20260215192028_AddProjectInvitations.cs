using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddProjectInvitations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectInvitations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(nullable: false),
                    StudentId = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    SentAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectInvitations_StudyProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "StudyProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectInvitations_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8ed94a10-d969-49be-8d42-17a06ab78c01", new DateTime(2026, 2, 15, 22, 20, 27, 895, DateTimeKind.Local).AddTicks(2108), "AQAAAAEAACcQAAAAENEYocyoPs2dNIjm/Jotc6DcxLUW0O9p1QaXVOK57sOGfOe62hwtl/KgWykEnv5xpA==", "0d035e9b-3568-4258-81e3-5a8f2f2c1495" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvitations_ProjectId",
                table: "ProjectInvitations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvitations_StudentId",
                table: "ProjectInvitations",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectInvitations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2fa84ca8-2b73-4dbd-9753-01a0f797db6b", new DateTime(2026, 2, 14, 18, 34, 42, 139, DateTimeKind.Local).AddTicks(4861), "AQAAAAEAACcQAAAAEL7qK02jo0O2RVCmZ7Uw7wlQVjl0ttYj0vBSgYvWWZuB3Lhrpgih6tZZPB9sY2JSuA==", "cd620de4-5d19-4d60-9be9-44f6b9c3bacf" });
        }
    }
}
