using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class SoftCopyFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Books",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Books",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0add8461-f6af-4f7e-a9c7-4dfeccdd34d2", "AQAAAAEAACcQAAAAEK3CbFK+7OUGMqc45t86T+EdcafAycUiWWY7Xy3wu0CNatgnqNptKAnJhSMF5HQttA==", "0badbfca-cc1f-4f02-8b97-6612f2ce67d2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Books");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fb7b5d15-dc89-4076-a44b-46773f609efc", "AQAAAAEAACcQAAAAECU+jeMV4TPvpxcstgPoXPpp70DQlKf8fptvF4qOTeJFtU09x1VtUnTDfbM/21R0+g==", "6d4af22d-5064-4457-97ca-a98f0bbc91d8" });
        }
    }
}
