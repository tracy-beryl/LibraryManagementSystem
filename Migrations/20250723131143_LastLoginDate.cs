using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class LastLoginDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b8b2ad4-02a0-4832-8503-3f15d71b17c6", new DateTime(2025, 7, 23, 16, 11, 42, 633, DateTimeKind.Local).AddTicks(4545), "AQAAAAEAACcQAAAAEAmL8zKAbmcYJlDefQhneoQ4qqxu/ciUxEi8MaqJyJqbqWvm6R+4dpcMYgm0xiJnxQ==", "d3a18d56-c4d0-4717-9ca1-1d6216fe6dcc" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4bff8591-0fae-4f40-ad10-212dad1e8d6e", new DateTime(2025, 7, 21, 12, 47, 11, 471, DateTimeKind.Local).AddTicks(2112), "AQAAAAEAACcQAAAAEP+XQ5o5fGoxb/W3wRiwyx49XvWVvqRsPpjdp1Kmqc6E0SK9BNeahRdVeiAL4K3ncg==", "f7388a74-88fa-4721-ab7c-ec1aad726b9e" });
        }
    }
}
