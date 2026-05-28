using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class ReferenceNumberandSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "Books",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce5adc3a-ab5d-43da-a701-edc915fcad54", new DateTime(2025, 7, 23, 20, 28, 28, 132, DateTimeKind.Local).AddTicks(4226), "AQAAAAEAACcQAAAAEBydfxY6iKb8lFFJFcisgpWknOVb/E8DnbGSvxynm6AsoesM2VL3mVL448iNeQJIEw==", "75f945aa-2556-4f97-b372-d2559d72a6f4" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "Books");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b8b2ad4-02a0-4832-8503-3f15d71b17c6", new DateTime(2025, 7, 23, 16, 11, 42, 633, DateTimeKind.Local).AddTicks(4545), "AQAAAAEAACcQAAAAEAmL8zKAbmcYJlDefQhneoQ4qqxu/ciUxEi8MaqJyJqbqWvm6R+4dpcMYgm0xiJnxQ==", "d3a18d56-c4d0-4717-9ca1-1d6216fe6dcc" });
        }
    }
}
