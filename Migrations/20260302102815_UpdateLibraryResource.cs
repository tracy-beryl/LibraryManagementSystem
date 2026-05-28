using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class UpdateLibraryResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "LibraryResources",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7473e9b6-b872-4d53-871f-97211c106314", new DateTime(2026, 3, 2, 13, 28, 14, 905, DateTimeKind.Local).AddTicks(4828), "AQAAAAEAACcQAAAAEMTefcRJ21gpSXKpOJAjQ/W6qMmMZUhhVLyZT8O7+jYqcq8Hgwce2YJxYQtXWCNL2g==", "852f7a89-5f7e-45d5-b83f-c85bc7e92ee8" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "LibraryResources");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9d2b0010-26ad-490b-a0fe-75191a879d1e", new DateTime(2026, 3, 2, 0, 38, 1, 71, DateTimeKind.Local).AddTicks(6207), "AQAAAAEAACcQAAAAEOOrzFH2W9tABJuox3UEoITnktK0KHSNMe4nljZpz4Ec5EMeXghu5Wlrv1GOmOOghw==", "3cac6dd8-89ba-40b1-b229-e7fa09832873" });
        }
    }
}
