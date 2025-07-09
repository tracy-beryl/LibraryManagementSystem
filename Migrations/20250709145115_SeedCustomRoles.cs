using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class SeedCustomRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "A1", "1" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "S1", "2" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "A1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "S1");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "FullName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoleId", "SecurityStamp", "StudentId", "TwoFactorEnabled", "UserName" },
                values: new object[] { "8b8b820f-bf84-4ffe-87bd-be619179d833", 0, "fb7b5d15-dc89-4076-a44b-46773f609efc", "johndoe@gmail.com", true, null, "John Doe", null, false, null, "JOHNDOE@GMAIL.COM", "JOHNDOE@GMAIL.COM", "AQAAAAEAACcQAAAAECU+jeMV4TPvpxcstgPoXPpp70DQlKf8fptvF4qOTeJFtU09x1VtUnTDfbM/21R0+g==", null, false, "e577164a-6f5f-4f2d-8f2d-f58bd6a1e8f8", "6d4af22d-5064-4457-97ca-a98f0bbc91d8", null, false, "johndoe@gmail.com" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsStudent", "RoleName" },
                values: new object[,]
                {
                    { "4b729351-7f04-4e51-9c7e-017fdf369e01", false, "Admin" },
                    { "5eba0279-cee8-40b6-92e9-fd54ba4a5b60", false, "Staff" },
                    { "25272b90-aa34-44d3-89cf-1ca24037ca92", true, "Student" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8b8b820f-bf84-4ffe-87bd-be619179d833");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "25272b90-aa34-44d3-89cf-1ca24037ca92");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "4b729351-7f04-4e51-9c7e-017fdf369e01");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "5eba0279-cee8-40b6-92e9-fd54ba4a5b60");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "c3a04f51-a4f8-420f-af0c-778afe613f72", "Admin", "ADMIN" },
                    { "2", "4d553da9-0c59-455e-999a-fe8bf7749ae6", "Staff", "STAFF" },
                    { "3", "a3cb5a9c-c09b-4a78-9311-e9b0c80c2cb5", "Student", "STUDENT" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "FullName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoleId", "SecurityStamp", "StudentId", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "A1", 0, "d9bf711c-e0ac-4b2e-8c92-32705cf01ae1", "johndoe@gmail.com", true, null, "John Doe", null, false, null, "JOHNDOE@GMAIL.COM", "JOHNDOE@GMAIL.COM", "AQAAAAEAACcQAAAAEMcCR7T5VePRv1n1+FmmyHXpOkz4jgfiGuouqfdjx/9p+z+cMnFB9IF5HUPaN6uj4Q==", null, false, null, "6c0cce2a-0661-4a50-b828-52a8f451e7aa", null, false, "johndoe@gmail.com" },
                    { "S1", 0, "a55a2124-892c-4caf-8c51-8fb90b3e0221", "moyimeso@gmail.com", true, null, "Moyi Meso", null, false, null, "MOYIMESO@GMAIL.COM", "MOYIMESO@GMAIL.COM", "AQAAAAEAACcQAAAAEMda1y0Yw8g0C5R8mj1emKWejgLKp1XYxXMAthP2fdsgE3Mzhq6NBPkTQBoPdJWbwQ==", null, false, null, "dc70c913-27f2-4648-9fcb-c71d456e6d5a", null, false, "moyimeso@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "A1", "1" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "S1", "2" });
        }
    }
}
