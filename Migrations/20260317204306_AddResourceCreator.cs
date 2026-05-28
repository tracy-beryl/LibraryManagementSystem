using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddResourceCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "LibraryResources",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4ae7658b-9085-48f0-9b25-b18ddd20799b", new DateTime(2026, 3, 17, 23, 43, 5, 317, DateTimeKind.Local).AddTicks(5036), "AQAAAAEAACcQAAAAECutYy6gVXmEC58qAT9yjxwGM5LMMuVcnhhGu8O0BNdRjJrVgOK7k/950Y5y+jNmXQ==", "a1a421c2-4239-49ca-b2c2-c686e6988b11" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "LibraryResources");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0811b73-5663-4668-a0a1-270a5c94ced3", new DateTime(2026, 3, 13, 21, 25, 33, 914, DateTimeKind.Local).AddTicks(6941), "AQAAAAEAACcQAAAAEN6jg8FmAaaiTqGlZRvyJfcd1MqeT3fZ2mgrTJPicWnU+7AQqWUDbtrcqwHZjeHcDw==", "3a59a637-16b4-4390-bb6c-3321cd6bb627" });
        }
    }
}
