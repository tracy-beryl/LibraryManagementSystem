using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class UpdateBookandApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "865162a5-c849-4218-aab3-3b0db8365ddd", new DateTime(2026, 3, 11, 0, 16, 45, 410, DateTimeKind.Local).AddTicks(9533), "AQAAAAEAACcQAAAAEJw4cnup5tJ9ceQxLfOBlgBLDNSWYg2YJXyjDV6TAzwPMAvN8eGLRWsKbsEJQAMDnA==", "3a0f0487-ffce-4a0e-aef0-4a039ba90b69" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aa225c23-b6be-4dfb-9239-479b62588309", new DateTime(2026, 3, 11, 0, 7, 44, 271, DateTimeKind.Local).AddTicks(3788), "AQAAAAEAACcQAAAAEPx9LYGl3atYML5a9CBtd8Q9qbl8YDy06Y4O4QhP5TRLz2gNi0nCfOChh9Mnq8fOQw==", "b1c8197f-cb72-4c86-89b0-3cea35e8bd31" });
        }
    }
}
