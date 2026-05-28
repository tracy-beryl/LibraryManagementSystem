using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddLastReadAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadAt",
                table: "ProjectMembers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1ff7cc38-e7e3-4fbf-90f5-ef6e7e2ddbf5", new DateTime(2026, 2, 21, 18, 54, 39, 216, DateTimeKind.Local).AddTicks(9489), "AQAAAAEAACcQAAAAEAyuMaiWZS15uOsH1vYd5oVEiqP2T61qmPVyVtkLVJpeGiAWAe5yOGwWou6BUAsYAg==", "592c7910-bec2-4e92-8540-d4242ff8bd16" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadAt",
                table: "ProjectMembers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "712bdb62-ee26-460a-8791-263d288fe7c6", new DateTime(2026, 2, 20, 23, 19, 40, 466, DateTimeKind.Local).AddTicks(8048), "AQAAAAEAACcQAAAAELmLGSKvekLfo83WY8B0932whj3kYh5m+ZQygRZmWR0cBDeOb/lLA/0Zxoy/pjB2cg==", "bbf67ad6-44fa-4a23-bd07-551bff957837" });
        }
    }
}
