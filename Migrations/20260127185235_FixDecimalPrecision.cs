using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class FixDecimalPrecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "09ebed1c-1526-4f69-a3d3-ba731705b3e4", new DateTime(2026, 1, 27, 21, 52, 34, 676, DateTimeKind.Local).AddTicks(8646), "AQAAAAEAACcQAAAAEH9H0sIJSpff86x+X9YQFCqDD+JWl/gD7EDGymOHqy7Qgf1t6kQpB1vI57ZV+HiVDQ==", "81c34990-d164-47b9-8e6d-c398df36a836" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1d2f0a3e-b42e-47e3-989f-960c831cf816", new DateTime(2026, 1, 27, 9, 25, 57, 6, DateTimeKind.Local).AddTicks(926), "AQAAAAEAACcQAAAAEJVVG5LqgeSqx7NNHdXE5lOjtrrAJFGcWFavIWZMkhAaeiw/JgePAjevKldPCeA7xg==", "f9690aab-0ed7-4313-97f7-05cda7143942" });
        }
    }
}
