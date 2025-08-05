using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddIsBorrowable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBorrowable",
                table: "Books",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4bff8591-0fae-4f40-ad10-212dad1e8d6e", new DateTime(2025, 7, 21, 12, 47, 11, 471, DateTimeKind.Local).AddTicks(2112), "AQAAAAEAACcQAAAAEP+XQ5o5fGoxb/W3wRiwyx49XvWVvqRsPpjdp1Kmqc6E0SK9BNeahRdVeiAL4K3ncg==", "f7388a74-88fa-4721-ab7c-ec1aad726b9e" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBorrowable",
                table: "Books");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "97b9587f-d383-4232-a1cf-851f65016bc1", new DateTime(2025, 7, 21, 8, 11, 18, 251, DateTimeKind.Local).AddTicks(1264), "AQAAAAEAACcQAAAAEIBQ481WIfGtqW8qGA3drFMQ+M17RIeiHghzToCISDiqUMr0Gx4Jfpjw22cKql/CEg==", "19a72b28-88bc-41f1-a47b-7d4faa488474" });
        }
    }
}
