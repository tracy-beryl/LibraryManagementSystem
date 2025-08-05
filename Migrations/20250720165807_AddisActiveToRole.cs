using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddisActiveToRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Roles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6e77e6b-3436-4fbc-b1e8-7e67d351c91c", "AQAAAAEAACcQAAAAELymp0Z63IW3earIKBTqqJZsAZKCUYyMO/YIIZvBpnqN+n5YKs2UkFyH1fklnb0vVw==", "e332cc84-818b-4141-887b-1c3d213e1542" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "25272b90-aa34-44d3-89cf-1ca24037ca92",
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "4b729351-7f04-4e51-9c7e-017fdf369e01",
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "5eba0279-cee8-40b6-92e9-fd54ba4a5b60",
                column: "IsActive",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Roles");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cf1b4acf-31a2-42e1-a56b-57dd0399a508", "AQAAAAEAACcQAAAAELa1WOL9CVPveuTkbfKU2o9/12y+wMIs8eeawK00prN0WmepIKUd45mtzbmYbgA+xQ==", "acb2319b-22b2-4665-895f-06024e3d95d4" });
        }
    }
}
