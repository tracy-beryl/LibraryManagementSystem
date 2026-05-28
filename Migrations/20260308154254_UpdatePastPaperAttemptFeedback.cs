using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class UpdatePastPaperAttemptFeedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PastPaperAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<int>(nullable: false),
                    StudentProfileId = table.Column<int>(nullable: false),
                    DifficultyRating = table.Column<int>(nullable: false),
                    ConfidenceRating = table.Column<int>(nullable: false),
                    ChallengingQuestions = table.Column<string>(nullable: true),
                    FeedbackNotes = table.Column<string>(nullable: true),
                    AttemptedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PastPaperAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PastPaperAttempts_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f5766f3b-fd17-4aac-a348-36f93d5aebcb", new DateTime(2026, 3, 8, 18, 42, 53, 309, DateTimeKind.Local).AddTicks(197), "AQAAAAEAACcQAAAAEACFwAaIanVsT5QCiRZ+Kvk3UGnmBUHMmlBz5t6kKea/q9DY6jeSlA07Xxtisv04pg==", "4dc4147e-aafd-403d-84a4-05b5b0e1310d" });

            migrationBuilder.CreateIndex(
                name: "IX_PastPaperAttempts_StudentProfileId",
                table: "PastPaperAttempts",
                column: "StudentProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PastPaperAttempts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7473e9b6-b872-4d53-871f-97211c106314", new DateTime(2026, 3, 2, 13, 28, 14, 905, DateTimeKind.Local).AddTicks(4828), "AQAAAAEAACcQAAAAEMTefcRJ21gpSXKpOJAjQ/W6qMmMZUhhVLyZT8O7+jYqcq8Hgwce2YJxYQtXWCNL2g==", "852f7a89-5f7e-45d5-b83f-c85bc7e92ee8" });
        }
    }
}
