using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class RefactorLoanFinePaymentFlow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FineAmountPaid",
                table: "Loans",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinePaidOn",
                table: "Loans",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinePaymentStatus",
                table: "Loans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MpesaPaymentPending",
                table: "Loans",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MpesaReceiptNumber",
                table: "Loans",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Loans",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionReference",
                table: "Loans",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "121d70de-fbbe-4fd7-812a-e3733d81da49", new DateTime(2026, 3, 23, 23, 22, 53, 533, DateTimeKind.Local).AddTicks(1602), "AQAAAAEAACcQAAAAEOQZXDFhLDFAdMK2rNw263UpZRgvDqDBjuxsprCa4wN7QAZkj207F/9XAY88aMm5Aw==", "a2ae48de-7944-455f-997d-19e47163ba6a" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FineAmountPaid",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "FinePaidOn",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "FinePaymentStatus",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "MpesaPaymentPending",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "MpesaReceiptNumber",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "Loans");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9699cede-d40a-4e7c-b1e6-589ff422a5d5", new DateTime(2026, 3, 19, 20, 14, 9, 866, DateTimeKind.Local).AddTicks(1573), "AQAAAAEAACcQAAAAEAK4pt5t946QJKI1wqxbAqUb5fEO3i+iO5MC4t/1bkYN5RNrLElQ9cq/ci5KFVSmJA==", "4b9eecc7-49ed-444b-bc80-2cc50fb469c4" });
        }
    }
}
