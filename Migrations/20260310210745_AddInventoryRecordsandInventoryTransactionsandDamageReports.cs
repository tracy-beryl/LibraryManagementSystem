using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddInventoryRecordsandInventoryTransactionsandDamageReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DamageReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(nullable: false),
                    ReportType = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    QuantityAffected = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    ReportedAt = table.Column<DateTime>(nullable: false),
                    ReportedByUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamageReports_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DamageReports_AspNetUsers_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(nullable: false),
                    TotalCopies = table.Column<int>(nullable: false),
                    AvailableCopies = table.Column<int>(nullable: false),
                    DamagedCopies = table.Column<int>(nullable: false),
                    MissingCopies = table.Column<int>(nullable: false),
                    ReorderThreshold = table.Column<int>(nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    LastUpdatedByUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRecords_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryRecords_AspNetUsers_LastUpdatedByUserId",
                        column: x => x.LastUpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryRecordId = table.Column<int>(nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    QuantityChanged = table.Column<int>(nullable: false),
                    PreviousAvailableCopies = table.Column<int>(nullable: false),
                    NewAvailableCopies = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    PerformedByUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_InventoryRecords_InventoryRecordId",
                        column: x => x.InventoryRecordId,
                        principalTable: "InventoryRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_AspNetUsers_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aa225c23-b6be-4dfb-9239-479b62588309", new DateTime(2026, 3, 11, 0, 7, 44, 271, DateTimeKind.Local).AddTicks(3788), "AQAAAAEAACcQAAAAEPx9LYGl3atYML5a9CBtd8Q9qbl8YDy06Y4O4QhP5TRLz2gNi0nCfOChh9Mnq8fOQw==", "b1c8197f-cb72-4c86-89b0-3cea35e8bd31" });

            migrationBuilder.CreateIndex(
                name: "IX_DamageReports_BookId",
                table: "DamageReports",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageReports_ReportedByUserId",
                table: "DamageReports",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRecords_BookId",
                table: "InventoryRecords",
                column: "BookId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRecords_LastUpdatedByUserId",
                table: "InventoryRecords",
                column: "LastUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_InventoryRecordId",
                table: "InventoryTransactions",
                column: "InventoryRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_PerformedByUserId",
                table: "InventoryTransactions",
                column: "PerformedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DamageReports");

            migrationBuilder.DropTable(
                name: "InventoryTransactions");

            migrationBuilder.DropTable(
                name: "InventoryRecords");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0fb2b921-baa4-4faa-8aca-b0712dbc25f0", new DateTime(2026, 3, 9, 15, 35, 7, 532, DateTimeKind.Local).AddTicks(4261), "AQAAAAEAACcQAAAAENantMylV8LxLAn1bDbg/dzNWcg6A4wjqEMl/Np+hnd1bLvepMEJiwgPjrmYdMo0zQ==", "b4d21575-e8e5-4db0-8d1d-96e84d1db6cb" });
        }
    }
}
