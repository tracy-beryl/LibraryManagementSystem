using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class PreventDuplicateBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "Books",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Edition",
                table: "Books",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "Books",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "56ba0601-4b1f-4a09-b738-18c20f1b89cb", new DateTime(2026, 4, 1, 10, 57, 28, 504, DateTimeKind.Local).AddTicks(3469), "AQAAAAEAACcQAAAAEBDgdYw3PP5OtNj1Eza3sCxdOHBHfyXRqil2qHp8M1Z+GYYMRuOCMnC+uu7VpMJXMA==", "9d39587f-3a5d-46da-a173-8125f36baaf5" });

            migrationBuilder.CreateIndex(
                name: "IX_Books_ISBN_Edition_Department",
                table: "Books",
                columns: new[] { "ISBN", "Edition", "Department" },
                unique: true,
                filter: "[Edition] IS NOT NULL AND [Department] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_ISBN_Edition_Department",
                table: "Books");

            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Edition",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2651300-6b7c-440e-807e-b810f9ac9253", new DateTime(2026, 3, 28, 22, 50, 20, 855, DateTimeKind.Local).AddTicks(1210), "AQAAAAEAACcQAAAAEOWiqvFtocjXsjuioawHou01HeBuWi9u0NIzD9yzRR02NNirfvmwJ6fOakreD+jeyQ==", "6005fd6c-603d-4ba9-b6e9-0eabe743fa07" });
        }
    }
}
