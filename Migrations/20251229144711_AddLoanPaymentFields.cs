using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddLoanPaymentFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckoutRequestId",
                table: "Loans",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Loans",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ReplacementCost",
                table: "Loans",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Loans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1811df84-fc66-4aaf-b9d5-f7f33a906a57", new DateTime(2025, 12, 29, 17, 47, 10, 388, DateTimeKind.Local).AddTicks(1723), "AQAAAAEAACcQAAAAEIcW1NMUIGWOCu/hJitISRKZy7lKc8BDQrsTcM/mg6zRMLtC7aldA9RQicF4jx1tuQ==", "780ac140-6b14-4c3c-9849-ffddc42bd979" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckoutRequestId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ReplacementCost",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Loans");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5763a183-41ef-4b05-860c-9f61f741a04f", new DateTime(2025, 12, 27, 15, 53, 43, 310, DateTimeKind.Local).AddTicks(814), "AQAAAAEAACcQAAAAEF7xO9YA6uZZjNjKEl+SNZYja8zXo9HIPsmSWdp/zfVT6nU6/ssAjqRbY6pOvcrY4Q==", "315d5af1-7243-40ff-8eb5-28190aba8388" });
        }
    }
}
