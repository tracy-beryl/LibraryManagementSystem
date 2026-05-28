using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddSoftCopy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "SoftCopies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    UploadedBy = table.Column<string>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftCopies", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c4679d25-5067-4c2d-85ec-7893920d7106", "AQAAAAEAACcQAAAAEAll2cWqNd+zbPSfDtgtfMAjS3R05+LX9K+k4N4e2PnjRJfNCbjiHUhj5VSfI8NzYQ==", "d6bd6b13-f8be-46d5-a35d-89aceb525f8f" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoftCopies");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6e77e6b-3436-4fbc-b1e8-7e67d351c91c", "AQAAAAEAACcQAAAAELymp0Z63IW3earIKBTqqJZsAZKCUYyMO/YIIZvBpnqN+n5YKs2UkFyH1fklnb0vVw==", "e332cc84-818b-4141-887b-1c3d213e1542" });
        }
    }
}
