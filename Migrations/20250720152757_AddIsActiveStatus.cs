using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddIsActiveStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "IsActive", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7dc77f98-c567-4f67-ac0f-1c6791062680", true, "AQAAAAEAACcQAAAAELhM2ApzgiWmB8nxo9s05h3BV/fN4PqKnLE8AT6ihTqFeyBw2LaUcWP9Spimh2MaCA==", "d209ec7e-f6df-47e4-b21d-06a7f480360b" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0add8461-f6af-4f7e-a9c7-4dfeccdd34d2", "AQAAAAEAACcQAAAAEK3CbFK+7OUGMqc45t86T+EdcafAycUiWWY7Xy3wu0CNatgnqNptKAnJhSMF5HQttA==", "0badbfca-cc1f-4f02-8b97-6612f2ce67d2" });
        }
    }
}
