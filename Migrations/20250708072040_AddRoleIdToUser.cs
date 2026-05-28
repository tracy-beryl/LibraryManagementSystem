using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddRoleIdToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "AspNetUsers",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "c2c0d38b-5bec-4f5f-a440-6ae02eb01d24");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "6267cc9c-9b19-4221-a42e-f9b6d722a914");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "fc9c9af3-8e1b-4dda-978b-92a0e82bdcaf");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "A1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8ca884df-a890-458a-8102-0864ef446362", "AQAAAAEAACcQAAAAEMNCIoao5OohS7SvsMaXJxi5hiKwf7vJshNw30bd8CgCHnT4SOSoONWsK68cmKcPWg==", "ec17a554-9c15-4088-94b7-0dfb57c59593" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "S1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "de89a69d-fbd1-42b8-81a5-97d533c7e7a0", "AQAAAAEAACcQAAAAEAc17+2bgAnaN5mJC7yzYCQ58QBqEu/49fLmD23imYC1S8gtnprdj+h6oUe6BG+ROA==", "51b3b633-2ec4-46a1-a771-d383aeed3a8a" });
        }
    }
}
