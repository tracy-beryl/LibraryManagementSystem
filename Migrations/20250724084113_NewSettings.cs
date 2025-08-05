using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class NewSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ShortCode = table.Column<string>(nullable: true),
                    LogoPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "786a581b-2b97-4995-abdc-196496e5f059", new DateTime(2025, 7, 24, 11, 41, 12, 456, DateTimeKind.Local).AddTicks(747), "AQAAAAEAACcQAAAAEM63R4yuKbdt7WtKeCQNuyaMgmw7EdmjFsZ18/hO268GZ6zNEZANgwfkaGaH0xNZbQ==", "470bde73-ad6e-4f33-93ae-51e999d02c3f" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2fd8c9b7-d1cc-4283-ada7-0ac9e0baa07d", new DateTime(2025, 7, 24, 11, 30, 13, 303, DateTimeKind.Local).AddTicks(6369), "AQAAAAEAACcQAAAAEF8RO2yrCtlyVxIWCzbFmFNF1hCuM1eUgGx9SGepHMtx+XfL44VGQGVPmsnTmksdEw==", "26917831-9d05-4dce-87c9-c3d5bd553028" });
        }
    }
}
