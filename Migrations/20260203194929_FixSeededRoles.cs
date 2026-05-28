using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class FixSeededRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.CreateTable(
                name: "BookSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    ISBN = table.Column<string>(nullable: true),
                    SuggestedByUserId = table.Column<string>(nullable: true),
                    SuggestedOn = table.Column<DateTime>(nullable: false),
                    Approved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookSuggestions_AspNetUsers_SuggestedByUserId",
                        column: x => x.SuggestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "RoleId", "SecurityStamp" },
                values: new object[] { "06e4be75-9677-4cb8-9c83-73f71b2fb8f7", new DateTime(2026, 2, 3, 22, 49, 28, 721, DateTimeKind.Local).AddTicks(4848), "AQAAAAEAACcQAAAAEPc6/1H3rPgQq0xQqPQzTxn6Fn03b4chC82/jtiomgI4ZDPsDK3BcPqDyF8pjM6Xsg==", null, "896a566b-65c5-469c-893f-8ba7fadc51c0" });

            migrationBuilder.CreateIndex(
                name: "IX_BookSuggestions_SuggestedByUserId",
                table: "BookSuggestions",
                column: "SuggestedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookSuggestions");

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "Id", "AdminName", "Email", "Password" },
                values: new object[,]
                {
                    { 1, "John Doe", "johndoe@gmail.com", null },
                    { 2, "Emily Harris", "emilyharris@gmail.com", null }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "RoleId", "SecurityStamp" },
                values: new object[] { "09ebed1c-1526-4f69-a3d3-ba731705b3e4", new DateTime(2026, 1, 27, 21, 52, 34, 676, DateTimeKind.Local).AddTicks(8646), "AQAAAAEAACcQAAAAEH9H0sIJSpff86x+X9YQFCqDD+JWl/gD7EDGymOHqy7Qgf1t6kQpB1vI57ZV+HiVDQ==", "e577164a-6f5f-4f2d-8f2d-f58bd6a1e8f8", "81c34990-d164-47b9-8e6d-c398df36a836" });

            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "Id", "Department", "Email", "Password", "StaffName" },
                values: new object[,]
                {
                    { 1, "Software Systems", "moyimeso@gmail.com", null, "Moyi Meso" },
                    { 2, "Information Systems", "leahkateb@gmail.com", null, "Leah Kateb" }
                });
        }
    }
}
