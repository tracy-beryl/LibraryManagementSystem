using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddPastPaperAttempts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "15d146b4-352f-47f4-875c-73dd353ee759", new DateTime(2026, 3, 8, 18, 57, 54, 144, DateTimeKind.Local).AddTicks(3135), "AQAAAAEAACcQAAAAEGaCY9tbs9exqf1Tf+3IyZAWtb0ICGDTzNQ9+e8zvz2SJywdYgHZrZFeKwFUqQLZHA==", "f0f18a4c-96ae-4205-83c6-ecfbe1e703d6" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f5766f3b-fd17-4aac-a348-36f93d5aebcb", new DateTime(2026, 3, 8, 18, 42, 53, 309, DateTimeKind.Local).AddTicks(197), "AQAAAAEAACcQAAAAEACFwAaIanVsT5QCiRZ+Kvk3UGnmBUHMmlBz5t6kKea/q9DY6jeSlA07Xxtisv04pg==", "4dc4147e-aafd-403d-84a4-05b5b0e1310d" });
        }
    }
}
