using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class FixDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FineAmountPaid",
                table: "Loans",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0a423d97-5cc1-44a2-bdef-1b1e6ba1d89a", new DateTime(2026, 3, 24, 0, 54, 59, 930, DateTimeKind.Local).AddTicks(4622), "AQAAAAEAACcQAAAAEM1ZLYS1+Dz1nQd19ruckk6KZILZbRZZsqhxKSuYbKAuUb+XQvVgEmaXlpoujcrRhg==", "abc8367e-831c-4056-bd70-eef266968bf2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FineAmountPaid",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "121d70de-fbbe-4fd7-812a-e3733d81da49", new DateTime(2026, 3, 23, 23, 22, 53, 533, DateTimeKind.Local).AddTicks(1602), "AQAAAAEAACcQAAAAEOQZXDFhLDFAdMK2rNw263UpZRgvDqDBjuxsprCa4wN7QAZkj207F/9XAY88aMm5Aw==", "a2ae48de-7944-455f-997d-19e47163ba6a" });
        }
    }
}
