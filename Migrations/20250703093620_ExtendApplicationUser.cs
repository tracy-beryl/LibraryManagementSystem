using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class ExtendApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "AspNetUsers",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "b8d4cea2-1103-4c45-858a-f0d1a1801e7f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "69b2cef6-b382-4a93-b8ec-f9d36eca069a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "085197f3-a890-4010-b8ce-cf8c669699e8");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "A1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b5947e00-85fa-4e19-ba88-f0bd05a97ed8", "AQAAAAEAACcQAAAAEDL3ztn5b6XcgI4dEbZWZ1T1K97BUC88Ow6Fh8foydoJiZsFLnwt2gJ2mNp09NdxUQ==", "ca11c6f5-1a03-495b-9a46-6e55aeeb269a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "S1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "741133fd-1969-4aba-9ae6-65401e38b1fb", "AQAAAAEAACcQAAAAECTc14J6i9MLYC/ADMNNJh2/vwvegmzpXD8Oq1tAtqFlM0+hLuA4bZ4V61erfUzbxw==", "e02d367e-dd38-427f-9723-6cb6ee99299a" });
        }
    }
}
