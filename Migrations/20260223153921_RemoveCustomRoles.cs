using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class RemoveCustomRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Roles_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AdmissionNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdentificationNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ResourceCompetencies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MappedAt",
                table: "ResourceCompetencies",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MappedByUserId",
                table: "ResourceCompetencies",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentProfileId",
                table: "Loans",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "CompetencyStandards",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "CompetencyStandards",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CompetencyStandards",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "CompetencyStandards",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VersionNumber",
                table: "CompetencyStandards",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StaffProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    StaffNumber = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    AdmissionNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f1f748a0-7881-40a3-a101-816f231b65f0", new DateTime(2026, 2, 23, 18, 39, 20, 394, DateTimeKind.Local).AddTicks(2923), "AQAAAAEAACcQAAAAEB/9HdwkNfQJUIsawqrdHoR0wtG2nGSF4OJab4OGIc1a+ChOPdD5dZScZb+qoNSisg==", "3495c147-0637-4b66-bc9c-9511ae17c2c1" });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCompetencies_MappedByUserId",
                table: "ResourceCompetencies",
                column: "MappedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_StudentProfileId",
                table: "Loans",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfiles_UserId",
                table: "StaffProfiles",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_UserId",
                table: "StudentProfiles",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_StudentProfiles_StudentProfileId",
                table: "Loans",
                column: "StudentProfileId",
                principalTable: "StudentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceCompetencies_AspNetUsers_MappedByUserId",
                table: "ResourceCompetencies",
                column: "MappedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_StudentProfiles_StudentProfileId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceCompetencies_AspNetUsers_MappedByUserId",
                table: "ResourceCompetencies");

            migrationBuilder.DropTable(
                name: "StaffProfiles");

            migrationBuilder.DropTable(
                name: "StudentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ResourceCompetencies_MappedByUserId",
                table: "ResourceCompetencies");

            migrationBuilder.DropIndex(
                name: "IX_Loans_StudentProfileId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ResourceCompetencies");

            migrationBuilder.DropColumn(
                name: "MappedAt",
                table: "ResourceCompetencies");

            migrationBuilder.DropColumn(
                name: "MappedByUserId",
                table: "ResourceCompetencies");

            migrationBuilder.DropColumn(
                name: "StudentProfileId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "CompetencyStandards");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "CompetencyStandards");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CompetencyStandards");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "CompetencyStandards");

            migrationBuilder.DropColumn(
                name: "VersionNumber",
                table: "CompetencyStandards");

            migrationBuilder.AddColumn<string>(
                name: "AdmissionNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsStudent = table.Column<bool>(type: "bit", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "063e63f2-caf1-4009-b2f5-61f5f31054f2", new DateTime(2026, 2, 22, 22, 31, 40, 256, DateTimeKind.Local).AddTicks(255), "AQAAAAEAACcQAAAAEGKyL++/uTARuGEyemx5q0nVrcAVhZ0Yr+063V/udL3pZIuvm6JwQqMoQneF5AkIvg==", "60c72c20-25b8-4643-ae24-3ad574f240de" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsActive", "IsStudent", "RoleName" },
                values: new object[,]
                {
                    { "4b729351-7f04-4e51-9c7e-017fdf369e01", true, false, "Admin" },
                    { "5eba0279-cee8-40b6-92e9-fd54ba4a5b60", true, false, "Staff" },
                    { "25272b90-aa34-44d3-89cf-1ca24037ca92", true, true, "Student" }
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
