using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddProjectDeadlineAndResources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDeadline_StudyProjects_StudyProjectId",
                table: "ProjectDeadline");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectResource_StudyProjects_StudyProjectId",
                table: "ProjectResource");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceView_ProjectResource_ProjectResourceId",
                table: "ResourceView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectResource",
                table: "ProjectResource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDeadline",
                table: "ProjectDeadline");

            migrationBuilder.RenameTable(
                name: "ProjectResource",
                newName: "ProjectResources");

            migrationBuilder.RenameTable(
                name: "ProjectDeadline",
                newName: "ProjectDeadlines");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectResource_StudyProjectId",
                table: "ProjectResources",
                newName: "IX_ProjectResources_StudyProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectDeadline_StudyProjectId",
                table: "ProjectDeadlines",
                newName: "IX_ProjectDeadlines_StudyProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectResources",
                table: "ProjectResources",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDeadlines",
                table: "ProjectDeadlines",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2fa84ca8-2b73-4dbd-9753-01a0f797db6b", new DateTime(2026, 2, 14, 18, 34, 42, 139, DateTimeKind.Local).AddTicks(4861), "AQAAAAEAACcQAAAAEL7qK02jo0O2RVCmZ7Uw7wlQVjl0ttYj0vBSgYvWWZuB3Lhrpgih6tZZPB9sY2JSuA==", "cd620de4-5d19-4d60-9be9-44f6b9c3bacf" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDeadlines_StudyProjects_StudyProjectId",
                table: "ProjectDeadlines",
                column: "StudyProjectId",
                principalTable: "StudyProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectResources_StudyProjects_StudyProjectId",
                table: "ProjectResources",
                column: "StudyProjectId",
                principalTable: "StudyProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceView_ProjectResources_ProjectResourceId",
                table: "ResourceView",
                column: "ProjectResourceId",
                principalTable: "ProjectResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDeadlines_StudyProjects_StudyProjectId",
                table: "ProjectDeadlines");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectResources_StudyProjects_StudyProjectId",
                table: "ProjectResources");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceView_ProjectResources_ProjectResourceId",
                table: "ResourceView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectResources",
                table: "ProjectResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDeadlines",
                table: "ProjectDeadlines");

            migrationBuilder.RenameTable(
                name: "ProjectResources",
                newName: "ProjectResource");

            migrationBuilder.RenameTable(
                name: "ProjectDeadlines",
                newName: "ProjectDeadline");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectResources_StudyProjectId",
                table: "ProjectResource",
                newName: "IX_ProjectResource_StudyProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectDeadlines_StudyProjectId",
                table: "ProjectDeadline",
                newName: "IX_ProjectDeadline_StudyProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectResource",
                table: "ProjectResource",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDeadline",
                table: "ProjectDeadline",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "13695a51-e39f-465d-93b3-c30b97877e26", new DateTime(2026, 2, 14, 16, 27, 3, 684, DateTimeKind.Local).AddTicks(7491), "AQAAAAEAACcQAAAAEG23rhw6SXiaPeB288+SVKlk+iSGFKLZT9t5UYe/uUhDkqG916lKv+fN0mr7Qt0t/A==", "61336ca5-b68a-4746-bf7a-2124fb69c702" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDeadline_StudyProjects_StudyProjectId",
                table: "ProjectDeadline",
                column: "StudyProjectId",
                principalTable: "StudyProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectResource_StudyProjects_StudyProjectId",
                table: "ProjectResource",
                column: "StudyProjectId",
                principalTable: "StudyProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceView_ProjectResource_ProjectResourceId",
                table: "ResourceView",
                column: "ProjectResourceId",
                principalTable: "ProjectResource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
