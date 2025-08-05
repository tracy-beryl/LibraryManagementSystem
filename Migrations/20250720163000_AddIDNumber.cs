using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddIDNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumber",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cf1b4acf-31a2-42e1-a56b-57dd0399a508", "AQAAAAEAACcQAAAAELa1WOL9CVPveuTkbfKU2o9/12y+wMIs8eeawK00prN0WmepIKUd45mtzbmYbgA+xQ==", "acb2319b-22b2-4665-895f-06024e3d95d4" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentificationNumber",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7dc77f98-c567-4f67-ac0f-1c6791062680", "AQAAAAEAACcQAAAAELhM2ApzgiWmB8nxo9s05h3BV/fN4PqKnLE8AT6ihTqFeyBw2LaUcWP9Spimh2MaCA==", "d209ec7e-f6df-47e4-b21d-06a7f480360b" });
        }
    }
}
