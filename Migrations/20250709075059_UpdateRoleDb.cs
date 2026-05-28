using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class UpdateRoleDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsStudent = table.Column<bool>(nullable: false),
                    RoleName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "c3a04f51-a4f8-420f-af0c-778afe613f72");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "4d553da9-0c59-455e-999a-fe8bf7749ae6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "a3cb5a9c-c09b-4a78-9311-e9b0c80c2cb5");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "A1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d9bf711c-e0ac-4b2e-8c92-32705cf01ae1", "AQAAAAEAACcQAAAAEMcCR7T5VePRv1n1+FmmyHXpOkz4jgfiGuouqfdjx/9p+z+cMnFB9IF5HUPaN6uj4Q==", "6c0cce2a-0661-4a50-b828-52a8f451e7aa" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "S1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a55a2124-892c-4caf-8c51-8fb90b3e0221", "AQAAAAEAACcQAAAAEMda1y0Yw8g0C5R8mj1emKWejgLKp1XYxXMAthP2fdsgE3Mzhq6NBPkTQBoPdJWbwQ==", "dc70c913-27f2-4648-9fcb-c71d456e6d5a" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "d759d7a1-021c-4e3b-aed7-c346c9e9e1cb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "af5168e3-49d5-4112-839e-2e86f569f223");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "40aa4503-a711-4a24-9103-1eccc39d922c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "A1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5a74dc0e-39b5-4e41-a428-ee4a85667363", "AQAAAAEAACcQAAAAEBdnNG1oyumCJh50Cv794EWsiqyX44UrfpbYzlZlmt16Rb1SvIvdoZqnMsaYKqFslQ==", "70290174-8b9a-4ec0-8fd9-1fdcc744c328" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "S1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "13d6c14b-41f3-4fcb-bc1a-c5c2ccba4eee", "AQAAAAEAACcQAAAAEL5MmQaCbSuRzYj+htnKcEGP4MlLrI01HBO87CAiX+5EzmNbCZG91tqf74yi2DIf1Q==", "c45fccde-cf58-48c0-b27b-4cda685d1202" });
        }
    }
}
