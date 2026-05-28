using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class UpdateResourceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectResources_StudyProjects_StudyProjectId",
                table: "ProjectResources");

            migrationBuilder.DropIndex(
                name: "IX_ProjectResources_StudyProjectId",
                table: "ProjectResources");

            migrationBuilder.DropColumn(
                name: "StudyProjectId",
                table: "ProjectResources");

            migrationBuilder.CreateTable(
                name: "CompetencyStandards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Program = table.Column<string>(nullable: true),
                    Level = table.Column<string>(nullable: true),
                    UnitCode = table.Column<string>(nullable: true),
                    UnitName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetencyStandards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UrlOrFilePath = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceCompetencies",
                columns: table => new
                {
                    ResourceId = table.Column<int>(nullable: false),
                    CompetencyStandardId = table.Column<int>(nullable: false),
                    ProjectResourceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCompetencies", x => new { x.ResourceId, x.CompetencyStandardId });
                    table.ForeignKey(
                        name: "FK_ResourceCompetencies_CompetencyStandards_CompetencyStandardId",
                        column: x => x.CompetencyStandardId,
                        principalTable: "CompetencyStandards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceCompetencies_ProjectResources_ProjectResourceId",
                        column: x => x.ProjectResourceId,
                        principalTable: "ProjectResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceCompetencies_LibraryResources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "LibraryResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "063e63f2-caf1-4009-b2f5-61f5f31054f2", new DateTime(2026, 2, 22, 22, 31, 40, 256, DateTimeKind.Local).AddTicks(255), "AQAAAAEAACcQAAAAEGKyL++/uTARuGEyemx5q0nVrcAVhZ0Yr+063V/udL3pZIuvm6JwQqMoQneF5AkIvg==", "60c72c20-25b8-4643-ae24-3ad574f240de" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectResources_ProjectId",
                table: "ProjectResources",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCompetencies_CompetencyStandardId",
                table: "ResourceCompetencies",
                column: "CompetencyStandardId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCompetencies_ProjectResourceId",
                table: "ResourceCompetencies",
                column: "ProjectResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectResources_StudyProjects_ProjectId",
                table: "ProjectResources",
                column: "ProjectId",
                principalTable: "StudyProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectResources_StudyProjects_ProjectId",
                table: "ProjectResources");

            migrationBuilder.DropTable(
                name: "ResourceCompetencies");

            migrationBuilder.DropTable(
                name: "CompetencyStandards");

            migrationBuilder.DropTable(
                name: "LibraryResources");

            migrationBuilder.DropIndex(
                name: "IX_ProjectResources_ProjectId",
                table: "ProjectResources");

            migrationBuilder.AddColumn<int>(
                name: "StudyProjectId",
                table: "ProjectResources",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7e21c3e6-1203-462e-a761-ca196cbfa75d", new DateTime(2026, 2, 22, 17, 8, 37, 246, DateTimeKind.Local).AddTicks(4727), "AQAAAAEAACcQAAAAEJ7ah0V+ZakPk5JoiD/1tmfKb/JjpB7Z0KETFgmldBXCQsZBxjP5ZKPbFZnhWHb93w==", "e19e37eb-d527-4b9e-b1e2-50266df77779" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectResources_StudyProjectId",
                table: "ProjectResources",
                column: "StudyProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectResources_StudyProjects_StudyProjectId",
                table: "ProjectResources",
                column: "StudyProjectId",
                principalTable: "StudyProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
