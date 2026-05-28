using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddBookFieldsToLibraryResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "LibraryResources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImagePath",
                table: "LibraryResources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "LibraryResources",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0fb2b921-baa4-4faa-8aca-b0712dbc25f0", new DateTime(2026, 3, 9, 15, 35, 7, 532, DateTimeKind.Local).AddTicks(4261), "AQAAAAEAACcQAAAAENantMylV8LxLAn1bDbg/dzNWcg6A4wjqEMl/Np+hnd1bLvepMEJiwgPjrmYdMo0zQ==", "b4d21575-e8e5-4db0-8d1d-96e84d1db6cb" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "LibraryResources");

            migrationBuilder.DropColumn(
                name: "CoverImagePath",
                table: "LibraryResources");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "LibraryResources");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "15d146b4-352f-47f4-875c-73dd353ee759", new DateTime(2026, 3, 8, 18, 57, 54, 144, DateTimeKind.Local).AddTicks(3135), "AQAAAAEAACcQAAAAEGaCY9tbs9exqf1Tf+3IyZAWtb0ICGDTzNQ9+e8zvz2SJywdYgHZrZFeKwFUqQLZHA==", "f0f18a4c-96ae-4205-83c6-ecfbe1e703d6" });
        }
    }
}
