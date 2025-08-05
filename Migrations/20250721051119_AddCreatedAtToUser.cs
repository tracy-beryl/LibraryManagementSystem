using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddCreatedAtToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "97b9587f-d383-4232-a1cf-851f65016bc1", new DateTime(2025, 7, 21, 8, 11, 18, 251, DateTimeKind.Local).AddTicks(1264), "AQAAAAEAACcQAAAAEIBQ481WIfGtqW8qGA3drFMQ+M17RIeiHghzToCISDiqUMr0Gx4Jfpjw22cKql/CEg==", "19a72b28-88bc-41f1-a47b-7d4faa488474" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c4679d25-5067-4c2d-85ec-7893920d7106", "AQAAAAEAACcQAAAAEAll2cWqNd+zbPSfDtgtfMAjS3R05+LX9K+k4N4e2PnjRJfNCbjiHUhj5VSfI8NzYQ==", "d6bd6b13-f8be-46d5-a35d-89aceb525f8f" });
        }
    }
}
