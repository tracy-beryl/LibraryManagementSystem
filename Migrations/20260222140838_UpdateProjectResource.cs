using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class UpdateProjectResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceView_ProjectResources_ProjectResourceId",
                table: "ResourceView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceView",
                table: "ResourceView");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ProjectResources");

            migrationBuilder.RenameTable(
                name: "ResourceView",
                newName: "ResourceViews");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceView_ProjectResourceId",
                table: "ResourceViews",
                newName: "IX_ResourceViews_ProjectResourceId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ProjectResources",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ProjectResources",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ProjectResources",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceViews",
                table: "ResourceViews",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7e21c3e6-1203-462e-a761-ca196cbfa75d", new DateTime(2026, 2, 22, 17, 8, 37, 246, DateTimeKind.Local).AddTicks(4727), "AQAAAAEAACcQAAAAEJ7ah0V+ZakPk5JoiD/1tmfKb/JjpB7Z0KETFgmldBXCQsZBxjP5ZKPbFZnhWHb93w==", "e19e37eb-d527-4b9e-b1e2-50266df77779" });

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceViews_ProjectResources_ProjectResourceId",
                table: "ResourceViews",
                column: "ProjectResourceId",
                principalTable: "ProjectResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceViews_ProjectResources_ProjectResourceId",
                table: "ResourceViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceViews",
                table: "ResourceViews");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ProjectResources");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProjectResources");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "ProjectResources");

            migrationBuilder.RenameTable(
                name: "ResourceViews",
                newName: "ResourceView");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceViews_ProjectResourceId",
                table: "ResourceView",
                newName: "IX_ResourceView_ProjectResourceId");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ProjectResources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceView",
                table: "ResourceView",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7fe8d00e-4701-4ec1-a6c7-6811d64c6581", new DateTime(2026, 2, 22, 0, 1, 2, 892, DateTimeKind.Local).AddTicks(9170), "AQAAAAEAACcQAAAAEAEQG23MZP+CewBKKGbteU6ZzbQScKRFiIEM6hPWPBsCmSm/mHQg3yEK1c+ucGugGA==", "1345edd7-c21a-48d7-84d6-569169d7b5fd" });

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceView_ProjectResources_ProjectResourceId",
                table: "ResourceView",
                column: "ProjectResourceId",
                principalTable: "ProjectResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
