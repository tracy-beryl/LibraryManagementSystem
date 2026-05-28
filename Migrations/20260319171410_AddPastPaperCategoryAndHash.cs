using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddPastPaperCategoryAndHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "PastPapers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FileHash",
                table: "PastPapers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "PastPapers",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9699cede-d40a-4e7c-b1e6-589ff422a5d5", new DateTime(2026, 3, 19, 20, 14, 9, 866, DateTimeKind.Local).AddTicks(1573), "AQAAAAEAACcQAAAAEAK4pt5t946QJKI1wqxbAqUb5fEO3i+iO5MC4t/1bkYN5RNrLElQ9cq/ci5KFVSmJA==", "4b9eecc7-49ed-444b-bc80-2cc50fb469c4" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "PastPapers");

            migrationBuilder.DropColumn(
                name: "FileHash",
                table: "PastPapers");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "PastPapers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4ae7658b-9085-48f0-9b25-b18ddd20799b", new DateTime(2026, 3, 17, 23, 43, 5, 317, DateTimeKind.Local).AddTicks(5036), "AQAAAAEAACcQAAAAECutYy6gVXmEC58qAT9yjxwGM5LMMuVcnhhGu8O0BNdRjJrVgOK7k/950Y5y+jNmXQ==", "a1a421c2-4239-49ca-b2c2-c686e6988b11" });
        }
    }
}
