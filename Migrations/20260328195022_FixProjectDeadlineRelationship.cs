using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class FixProjectDeadlineRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDeadlines_StudyProjects_StudyProjectId",
                table: "ProjectDeadlines");

            migrationBuilder.DropIndex(
                name: "IX_ProjectDeadlines_StudyProjectId",
                table: "ProjectDeadlines");

            migrationBuilder.DropColumn(
                name: "StudyProjectId",
                table: "ProjectDeadlines");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ProjectDeadlines",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2651300-6b7c-440e-807e-b810f9ac9253", new DateTime(2026, 3, 28, 22, 50, 20, 855, DateTimeKind.Local).AddTicks(1210), "AQAAAAEAACcQAAAAEOWiqvFtocjXsjuioawHou01HeBuWi9u0NIzD9yzRR02NNirfvmwJ6fOakreD+jeyQ==", "6005fd6c-603d-4ba9-b6e9-0eabe743fa07" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeadlines_ProjectId",
                table: "ProjectDeadlines",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDeadlines_StudyProjects_ProjectId",
                table: "ProjectDeadlines",
                column: "ProjectId",
                principalTable: "StudyProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDeadlines_StudyProjects_ProjectId",
                table: "ProjectDeadlines");

            migrationBuilder.DropIndex(
                name: "IX_ProjectDeadlines_ProjectId",
                table: "ProjectDeadlines");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ProjectDeadlines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "StudyProjectId",
                table: "ProjectDeadlines",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0a423d97-5cc1-44a2-bdef-1b1e6ba1d89a", new DateTime(2026, 3, 24, 0, 54, 59, 930, DateTimeKind.Local).AddTicks(4622), "AQAAAAEAACcQAAAAEM1ZLYS1+Dz1nQd19ruckk6KZILZbRZZsqhxKSuYbKAuUb+XQvVgEmaXlpoujcrRhg==", "abc8367e-831c-4056-bd70-eef266968bf2" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeadlines_StudyProjectId",
                table: "ProjectDeadlines",
                column: "StudyProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDeadlines_StudyProjects_StudyProjectId",
                table: "ProjectDeadlines",
                column: "StudyProjectId",
                principalTable: "StudyProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
