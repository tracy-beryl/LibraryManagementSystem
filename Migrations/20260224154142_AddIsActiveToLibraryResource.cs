using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddIsActiveToLibraryResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LibraryResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4d0cd2cb-eb87-449a-9dc9-983f89feceb9", new DateTime(2026, 2, 24, 18, 41, 42, 1, DateTimeKind.Local).AddTicks(3358), "AQAAAAEAACcQAAAAEIGUyrjSQOnp+dxJuYD/b/oAnI5fQpNPeZ7C/uJuqaM4ang+9/IdNCWSX+KctXOYng==", "7b18f77f-8786-4a75-b169-1f5d525febc9" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LibraryResources");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "133d2536-6254-4dea-95de-8e07fe7dee1d", new DateTime(2026, 2, 24, 1, 48, 35, 750, DateTimeKind.Local).AddTicks(5638), "AQAAAAEAACcQAAAAEKAs0EOKvxegF9eryG5zgYmNC/VHeshJY6moFgLrSpmOIL19JgYyqTcyX/8pzP/g7w==", "51a711f1-e24e-4d3f-8083-8e593c338f1e" });
        }
    }
}
