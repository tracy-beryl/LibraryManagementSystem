using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddStudyProjectModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdmissionNumber",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudyProjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyProjects_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDeadline",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    DueDate = table.Column<DateTime>(nullable: false),
                    StudyProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDeadline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectDeadline_StudyProjects_StudyProjectId",
                        column: x => x.StudyProjectId,
                        principalTable: "StudyProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMembers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(nullable: false),
                    StudentId = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_StudyProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "StudyProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectResource",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    UploadedById = table.Column<string>(nullable: true),
                    UploadedAt = table.Column<DateTime>(nullable: false),
                    StudyProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectResource_StudyProjects_StudyProjectId",
                        column: x => x.StudyProjectId,
                        principalTable: "StudyProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResourceView",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<int>(nullable: false),
                    StudentId = table.Column<string>(nullable: true),
                    ViewedAt = table.Column<DateTime>(nullable: false),
                    ProjectResourceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceView", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceView_ProjectResource_ProjectResourceId",
                        column: x => x.ProjectResourceId,
                        principalTable: "ProjectResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "13695a51-e39f-465d-93b3-c30b97877e26", new DateTime(2026, 2, 14, 16, 27, 3, 684, DateTimeKind.Local).AddTicks(7491), "AQAAAAEAACcQAAAAEG23rhw6SXiaPeB288+SVKlk+iSGFKLZT9t5UYe/uUhDkqG916lKv+fN0mr7Qt0t/A==", "61336ca5-b68a-4746-bf7a-2124fb69c702" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeadline_StudyProjectId",
                table: "ProjectDeadline",
                column: "StudyProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_ProjectId",
                table: "ProjectMembers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_StudentId",
                table: "ProjectMembers",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectResource_StudyProjectId",
                table: "ProjectResource",
                column: "StudyProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceView_ProjectResourceId",
                table: "ResourceView",
                column: "ProjectResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyProjects_OwnerId",
                table: "StudyProjects",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectDeadline");

            migrationBuilder.DropTable(
                name: "ProjectMembers");

            migrationBuilder.DropTable(
                name: "ResourceView");

            migrationBuilder.DropTable(
                name: "ProjectResource");

            migrationBuilder.DropTable(
                name: "StudyProjects");

            migrationBuilder.DropColumn(
                name: "AdmissionNumber",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b3759c7-abed-4d6f-9f0e-a511aaa9bd59", new DateTime(2026, 2, 4, 21, 43, 22, 223, DateTimeKind.Local).AddTicks(3099), "AQAAAAEAACcQAAAAEO3gf3+E2WebwR39/DySbdYSaprPiiJ7wYpTtJDrVF8coB7tG6wj92emRn/XXbg3vQ==", "62bc35bf-1f16-46a1-9bda-549d28875f14" });
        }
    }
}
