using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddFrozenFineAmountToLoan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Loans");

            migrationBuilder.AddColumn<decimal>(
                name: "FrozenFineAmount",
                table: "Loans",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5b511656-5a78-4f68-9b64-cb42f7418eca", new DateTime(2026, 1, 4, 22, 41, 13, 756, DateTimeKind.Local).AddTicks(6761), "AQAAAAEAACcQAAAAEMiXfrjW9JjVfd+6I5hrMfunXg6k/XX8yAftJeLK3x/MYDmt1R0ouhF7r3rdXECprQ==", "d768a024-aa13-43df-a5da-c7dcc7a38452" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrozenFineAmount",
                table: "Loans");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a2356875-a14b-48c7-ae77-cd63c5a47971", new DateTime(2026, 1, 3, 18, 26, 32, 12, DateTimeKind.Local).AddTicks(8885), "AQAAAAEAACcQAAAAEEARpqqqG5qIj3ibMbBj+ce5ZiDy7/4KpefB0MxICAGuXh0+eNvCITS9xJWfXJNbqg==", "b4b64435-1cfd-4f15-977b-49b9d68f08ac" });
        }
    }
}
