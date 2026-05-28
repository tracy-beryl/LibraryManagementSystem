using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class UpdateStudentProgressAndLibraryResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttemptCount",
                table: "StudentResourceProgresses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PageCoveragePercent",
                table: "StudentResourceProgresses",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "QuizScore",
                table: "StudentResourceProgresses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TimeSpentSeconds",
                table: "StudentResourceProgresses",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WatchSeconds",
                table: "StudentResourceProgresses",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DurationSeconds",
                table: "LibraryResources",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9d2b0010-26ad-490b-a0fe-75191a879d1e", new DateTime(2026, 3, 2, 0, 38, 1, 71, DateTimeKind.Local).AddTicks(6207), "AQAAAAEAACcQAAAAEOOrzFH2W9tABJuox3UEoITnktK0KHSNMe4nljZpz4Ec5EMeXghu5Wlrv1GOmOOghw==", "3cac6dd8-89ba-40b1-b229-e7fa09832873" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptCount",
                table: "StudentResourceProgresses");

            migrationBuilder.DropColumn(
                name: "PageCoveragePercent",
                table: "StudentResourceProgresses");

            migrationBuilder.DropColumn(
                name: "QuizScore",
                table: "StudentResourceProgresses");

            migrationBuilder.DropColumn(
                name: "TimeSpentSeconds",
                table: "StudentResourceProgresses");

            migrationBuilder.DropColumn(
                name: "WatchSeconds",
                table: "StudentResourceProgresses");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "LibraryResources");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6e588ed-980a-4f74-a5fa-d36f468fbf24", new DateTime(2026, 2, 26, 10, 19, 49, 496, DateTimeKind.Local).AddTicks(8137), "AQAAAAEAACcQAAAAEDgL5oEZWn9bO1QRvZFwMUaKNg0ZX7QX/9yScoylaq+AhmTx7QYUAWqbXJEh+GnH+Q==", "7bc30eb9-0ca9-4839-9103-e6350818f2e3" });
        }
    }
}
