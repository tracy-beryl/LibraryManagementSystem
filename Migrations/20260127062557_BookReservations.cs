using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class BookReservations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookReservations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ReservedOn = table.Column<DateTime>(nullable: false),
                    IsFulfilled = table.Column<bool>(nullable: false),
                    IsNotified = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookReservations_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookReservations_AspNetUsers_UserId",
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
                values: new object[] { "1d2f0a3e-b42e-47e3-989f-960c831cf816", new DateTime(2026, 1, 27, 9, 25, 57, 6, DateTimeKind.Local).AddTicks(926), "AQAAAAEAACcQAAAAEJVVG5LqgeSqx7NNHdXE5lOjtrrAJFGcWFavIWZMkhAaeiw/JgePAjevKldPCeA7xg==", "f9690aab-0ed7-4313-97f7-05cda7143942" });

            migrationBuilder.CreateIndex(
                name: "IX_BookReservations_BookId",
                table: "BookReservations",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookReservations_UserId",
                table: "BookReservations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookReservations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5b511656-5a78-4f68-9b64-cb42f7418eca", new DateTime(2026, 1, 4, 22, 41, 13, 756, DateTimeKind.Local).AddTicks(6761), "AQAAAAEAACcQAAAAEMiXfrjW9JjVfd+6I5hrMfunXg6k/XX8yAftJeLK3x/MYDmt1R0ouhF7r3rdXECprQ==", "d768a024-aa13-43df-a5da-c7dcc7a38452" });
        }
    }
}
