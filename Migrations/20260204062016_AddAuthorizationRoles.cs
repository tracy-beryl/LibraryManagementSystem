using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddAuthorizationRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "014efdbc-0840-4efa-b761-8a573e8f3029", new DateTime(2026, 2, 4, 9, 20, 16, 110, DateTimeKind.Local).AddTicks(8732), "AQAAAAEAACcQAAAAEIHv5rqeVPVr7zz2CXxMEX0+vRsF63txLhJUE/Nawx7k46KIhmn+Di1h1t7PHi5u3g==", "cc7f0f78-67a3-4954-b531-0990dd6a0c36" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RoleId",
                table: "AspNetUsers",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Roles_RoleId",
                table: "AspNetUsers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Roles_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RoleId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "06e4be75-9677-4cb8-9c83-73f71b2fb8f7", new DateTime(2026, 2, 3, 22, 49, 28, 721, DateTimeKind.Local).AddTicks(4848), "AQAAAAEAACcQAAAAEPc6/1H3rPgQq0xQqPQzTxn6Fn03b4chC82/jtiomgI4ZDPsDK3BcPqDyF8pjM6Xsg==", "896a566b-65c5-469c-893f-8ba7fadc51c0" });
        }
    }
}
