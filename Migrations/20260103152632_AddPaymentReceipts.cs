using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddPaymentReceipts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    CheckoutRequestId = table.Column<string>(nullable: true),
                    PaidOn = table.Column<DateTime>(nullable: false),
                    Method = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentReceipts_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a2356875-a14b-48c7-ae77-cd63c5a47971", new DateTime(2026, 1, 3, 18, 26, 32, 12, DateTimeKind.Local).AddTicks(8885), "AQAAAAEAACcQAAAAEEARpqqqG5qIj3ibMbBj+ce5ZiDy7/4KpefB0MxICAGuXh0+eNvCITS9xJWfXJNbqg==", "b4b64435-1cfd-4f15-977b-49b9d68f08ac" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_LoanId",
                table: "PaymentReceipts",
                column: "LoanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentReceipts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1811df84-fc66-4aaf-b9d5-f7f33a906a57", new DateTime(2025, 12, 29, 17, 47, 10, 388, DateTimeKind.Local).AddTicks(1723), "AQAAAAEAACcQAAAAEIcW1NMUIGWOCu/hJitISRKZy7lKc8BDQrsTcM/mg6zRMLtC7aldA9RQicF4jx1tuQ==", "780ac140-6b14-4c3c-9849-ffddc42bd979" });
        }
    }
}
