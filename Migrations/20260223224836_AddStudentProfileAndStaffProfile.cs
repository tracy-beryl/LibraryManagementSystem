using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddStudentProfileAndStaffProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "StudentProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Program",
                table: "StudentProfiles",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "133d2536-6254-4dea-95de-8e07fe7dee1d", new DateTime(2026, 2, 24, 1, 48, 35, 750, DateTimeKind.Local).AddTicks(5638), "AQAAAAEAACcQAAAAEKAs0EOKvxegF9eryG5zgYmNC/VHeshJY6moFgLrSpmOIL19JgYyqTcyX/8pzP/g7w==", "51a711f1-e24e-4d3f-8083-8e593c338f1e" });

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Program",
                table: "StudentProfiles");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f1f748a0-7881-40a3-a101-816f231b65f0", new DateTime(2026, 2, 23, 18, 39, 20, 394, DateTimeKind.Local).AddTicks(2923), "AQAAAAEAACcQAAAAEB/9HdwkNfQJUIsawqrdHoR0wtG2nGSF4OJab4OGIc1a+ChOPdD5dZScZb+qoNSisg==", "3495c147-0637-4b66-bc9c-9511ae17c2c1" });

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
