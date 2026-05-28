using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "Settings",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Settings",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Books",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Edition",
                table: "Books",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShelfNumber",
                table: "Books",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5763a183-41ef-4b05-860c-9f61f741a04f", new DateTime(2025, 12, 27, 15, 53, 43, 310, DateTimeKind.Local).AddTicks(814), "AQAAAAEAACcQAAAAEF7xO9YA6uZZjNjKEl+SNZYja8zXo9HIPsmSWdp/zfVT6nU6/ssAjqRbY6pOvcrY4Q==", "315d5af1-7243-40ff-8eb5-28190aba8388" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Edition",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ShelfNumber",
                table: "Books");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "786a581b-2b97-4995-abdc-196496e5f059", new DateTime(2025, 7, 24, 11, 41, 12, 456, DateTimeKind.Local).AddTicks(747), "AQAAAAEAACcQAAAAEM63R4yuKbdt7WtKeCQNuyaMgmw7EdmjFsZ18/hO268GZ6zNEZANgwfkaGaH0xNZbQ==", "470bde73-ad6e-4f33-93ae-51e999d02c3f" });
        }
    }
}
