using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class Settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2fd8c9b7-d1cc-4283-ada7-0ac9e0baa07d", new DateTime(2025, 7, 24, 11, 30, 13, 303, DateTimeKind.Local).AddTicks(6369), "AQAAAAEAACcQAAAAEF8RO2yrCtlyVxIWCzbFmFNF1hCuM1eUgGx9SGepHMtx+XfL44VGQGVPmsnTmksdEw==", "26917831-9d05-4dce-87c9-c3d5bd553028" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce5adc3a-ab5d-43da-a701-edc915fcad54", new DateTime(2025, 7, 23, 20, 28, 28, 132, DateTimeKind.Local).AddTicks(4226), "AQAAAAEAACcQAAAAEBydfxY6iKb8lFFJFcisgpWknOVb/E8DnbGSvxynm6AsoesM2VL3mVL448iNeQJIEw==", "75f945aa-2556-4f97-b372-d2559d72a6f4" });
        }
    }
}
