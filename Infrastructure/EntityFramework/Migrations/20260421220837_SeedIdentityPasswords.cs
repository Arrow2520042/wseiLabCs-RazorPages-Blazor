using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6D89BC32-15F1-4E30-AF25-53F1B4429A10",
                columns: new[] { "LockoutEnabled", "PasswordHash" },
                values: new object[] { true, "AQAAAAIAAYagAAAAEIUGp/TT1NcVE8M77dDN1R49jG6hYMnAlyOzNFVZsFbxYLXF7djmW9FsFcIh1UyZ5A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6D89BC32-15F1-4E30-AF25-53F1B4429A11",
                columns: new[] { "LockoutEnabled", "PasswordHash" },
                values: new object[] { true, "AQAAAAIAAYagAAAAEMDVL814x0HYV8B6rILlR6sZjGfiS7H/0kQZNVMeYfsLd0pg/nFCQmr7/8zlW6NaEw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6D89BC32-15F1-4E30-AF25-53F1B4429A10",
                columns: new[] { "LockoutEnabled", "PasswordHash" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6D89BC32-15F1-4E30-AF25-53F1B4429A11",
                columns: new[] { "LockoutEnabled", "PasswordHash" },
                values: new object[] { false, null });
        }
    }
}
