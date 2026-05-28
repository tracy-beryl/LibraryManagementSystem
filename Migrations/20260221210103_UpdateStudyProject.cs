using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class UpdateStudyProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7fe8d00e-4701-4ec1-a6c7-6811d64c6581", new DateTime(2026, 2, 22, 0, 1, 2, 892, DateTimeKind.Local).AddTicks(9170), "AQAAAAEAACcQAAAAEAEQG23MZP+CewBKKGbteU6ZzbQScKRFiIEM6hPWPBsCmSm/mHQg3yEK1c+ucGugGA==", "1345edd7-c21a-48d7-84d6-569169d7b5fd" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1ff7cc38-e7e3-4fbf-90f5-ef6e7e2ddbf5", new DateTime(2026, 2, 21, 18, 54, 39, 216, DateTimeKind.Local).AddTicks(9489), "AQAAAAEAACcQAAAAEAyuMaiWZS15uOsH1vYd5oVEiqP2T61qmPVyVtkLVJpeGiAWAe5yOGwWou6BUAsYAg==", "592c7910-bec2-4e92-8540-d4242ff8bd16" });
        }
    }
}
