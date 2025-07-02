using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class PastPapers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "PastPapers");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "PastPapers");

            migrationBuilder.AlterColumn<string>(
                name: "CourseCode",
                table: "PastPapers",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcademicYear",
                table: "PastPapers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CourseTitle",
                table: "PastPapers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Semester",
                table: "PastPapers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "PastPapers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "PastPapers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "b8d4cea2-1103-4c45-858a-f0d1a1801e7f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "69b2cef6-b382-4a93-b8ec-f9d36eca069a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "085197f3-a890-4010-b8ce-cf8c669699e8");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "A1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b5947e00-85fa-4e19-ba88-f0bd05a97ed8", "AQAAAAEAACcQAAAAEDL3ztn5b6XcgI4dEbZWZ1T1K97BUC88Ow6Fh8foydoJiZsFLnwt2gJ2mNp09NdxUQ==", "ca11c6f5-1a03-495b-9a46-6e55aeeb269a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "S1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "741133fd-1969-4aba-9ae6-65401e38b1fb", "AQAAAAEAACcQAAAAECTc14J6i9MLYC/ADMNNJh2/vwvegmzpXD8Oq1tAtqFlM0+hLuA4bZ4V61erfUzbxw==", "e02d367e-dd38-427f-9723-6cb6ee99299a" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademicYear",
                table: "PastPapers");

            migrationBuilder.DropColumn(
                name: "CourseTitle",
                table: "PastPapers");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "PastPapers");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "PastPapers");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "PastPapers");

            migrationBuilder.AlterColumn<string>(
                name: "CourseCode",
                table: "PastPapers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "PastPapers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "PastPapers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "1ff02d6b-2b5b-412c-9d26-c66b0f2b45e7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "22eb2965-f33c-453e-b6ac-ede3d0763f3a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "71bf6b5b-66d3-444d-8833-941fb8ea2b6c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "A1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6144b20-79d8-4ffb-85bd-b0d30ed9bcf8", "AQAAAAEAACcQAAAAEDyzR+sx5YId95cWAIGmgxomxa13j7nMCpnFn9ZabXSuJ1rrlwov4WCfzBpjgrQZXA==", "9824437f-1c73-4655-a0da-386331cd192c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "S1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "06cdc227-e599-4896-b299-1348fb3299a7", "AQAAAAEAACcQAAAAEOltMYV36NScRwb4RLZykxrOk3TAbhAUu2m3HrHN/a7lSEU2ERch4KjE/mDCOa70HA==", "71c7c5f4-7fc4-4add-b3d8-58e043e626d4" });
        }
    }
}
