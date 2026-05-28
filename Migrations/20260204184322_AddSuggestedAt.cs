using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddSuggestedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SuggestedAt",
                table: "BookSuggestions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b3759c7-abed-4d6f-9f0e-a511aaa9bd59", new DateTime(2026, 2, 4, 21, 43, 22, 223, DateTimeKind.Local).AddTicks(3099), "AQAAAAEAACcQAAAAEO3gf3+E2WebwR39/DySbdYSaprPiiJ7wYpTtJDrVF8coB7tG6wj92emRn/XXbg3vQ==", "62bc35bf-1f16-46a1-9bda-549d28875f14" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuggestedAt",
                table: "BookSuggestions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "014efdbc-0840-4efa-b761-8a573e8f3029", new DateTime(2026, 2, 4, 9, 20, 16, 110, DateTimeKind.Local).AddTicks(8732), "AQAAAAEAACcQAAAAEIHv5rqeVPVr7zz2CXxMEX0+vRsF63txLhJUE/Nawx7k46KIhmn+Di1h1t7PHi5u3g==", "cc7f0f78-67a3-4954-b531-0990dd6a0c36" });
        }
    }
}
