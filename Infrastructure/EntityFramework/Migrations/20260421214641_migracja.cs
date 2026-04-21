using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class migracja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeactivatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Address_Street = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Address_City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Address_PostalCode = table.Column<string>(type: "TEXT", maxLength: 12, nullable: true),
                    Address_Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Address_Type = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContactType = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Nip = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Organization_Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Position = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    EmployerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Contacts_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Contacts_Contacts_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB10", null, "System administrator", "Administrator", "ADMINISTRATOR" },
                    { "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB11", null, "Sales manager", "SalesManager", "SALESMANAGER" },
                    { "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB12", null, "Sales representative", "Salesperson", "SALESPERSON" },
                    { "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB13", null, "Support employee", "SupportAgent", "SUPPORTAGENT" },
                    { "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB14", null, "Read-only access", "ReadOnly", "READONLY" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DeactivatedAt", "Department", "Email", "EmailConfirmed", "FirstName", "FullName", "LastLoginAt", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Status", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "6D89BC32-15F1-4E30-AF25-53F1B4429A10", 0, "CON-CRM-ADMIN-001", new DateTime(2026, 3, 31, 8, 0, 0, 0, DateTimeKind.Utc), null, "IT", "admin@wsei.edu.pl", true, "System", "System Admin", null, "Admin", false, null, "ADMIN@WSEI.EDU.PL", "ADMIN@WSEI.EDU.PL", null, null, false, "SEC-CRM-ADMIN-001", "Active", false, "admin@wsei.edu.pl" },
                    { "6D89BC32-15F1-4E30-AF25-53F1B4429A11", 0, "CON-CRM-SALES-001", new DateTime(2026, 3, 31, 8, 0, 0, 0, DateTimeKind.Utc), null, "Sales", "sales@wsei.edu.pl", true, "Sales", "Sales Manager", null, "Manager", false, null, "SALES@WSEI.EDU.PL", "SALES@WSEI.EDU.PL", null, null, false, "SEC-CRM-SALES-001", "Active", false, "sales@wsei.edu.pl" }
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "ContactType", "CreatedAt", "Email", "Name", "Nip", "Notes", "Phone", "Status", "Tags", "Website" },
                values: new object[] { new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e11"), "Company", new DateTime(2026, 4, 20, 9, 30, 0, 0, DateTimeKind.Utc), "kontakt@balticsoft.pl", "Baltic Soft Solutions", "5842781123", "[{\"Content\":\"Klient strategiczny z Pomorza.\",\"CreatedAt\":\"2026-04-20T09:30:00Z\",\"Id\":\"8a8ecde7-0eaa-4e9e-a67f-3ac9857f5011\"}]", "583221104", "Active", "[\"b2b\",\"it\",\"partner\"]", "https://balticsoft.pl" });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "ContactType", "CreatedAt", "Description", "Email", "Organization_Name", "Notes", "Phone", "Status", "Tags", "Type" },
                values: new object[] { new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e12"), "Organization", new DateTime(2026, 4, 20, 9, 30, 0, 0, DateTimeKind.Utc), "Siec wspolpracy firm technologicznych.", "biuro@izbacyfrowa.pl", "Pomorska Izba Cyfrowa", "[]", "583004455", "Active", "[\"networking\",\"wydarzenia\"]", "Other" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB10", "6D89BC32-15F1-4E30-AF25-53F1B4429A10" },
                    { "6B4EE8AB-8BA8-4DDF-8E8E-CC0AF1D3AB11", "6D89BC32-15F1-4E30-AF25-53F1B4429A11" }
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "Address_City", "Address_Country", "Address_PostalCode", "Address_Street", "Address_Type", "BirthDate", "ContactType", "CreatedAt", "Email", "EmployerId", "FirstName", "Gender", "LastName", "Notes", "OrganizationId", "Phone", "Position", "Status", "Tags" },
                values: new object[,]
                {
                    { new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e21"), "Gdansk", "Polska", "80-244", "ul. Grunwaldzka 103A", "Work", new DateTime(1993, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Person", new DateTime(2026, 4, 20, 9, 30, 0, 0, DateTimeKind.Utc), "k.witkowska@balticsoft.pl", new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e11"), "Katarzyna", "Female", "Witkowska", "[{\"Content\":\"Preferuje kontakt telefoniczny rano.\",\"CreatedAt\":\"2026-04-20T09:30:00Z\",\"Id\":\"8a8ecde7-0eaa-4e9e-a67f-3ac9857f6011\"}]", new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e12"), "501778992", "Kierownik Projektu", "Active", "[\"decision-maker\",\"enterprise\"]" },
                    { new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e22"), "Warszawa", "Polska", "00-141", "al. Jana Pawla II 28", "Home", new DateTime(1990, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Person", new DateTime(2026, 4, 20, 9, 30, 0, 0, DateTimeKind.Utc), "m.czernecki@balticsoft.pl", new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e11"), "Michal", "Male", "Czernecki", "[]", new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e12"), "603115870", "Architekt Rozwiazan", "Active", "[\"security\",\"cloud\"]" },
                    { new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e23"), "Lodz", "Polska", "90-051", "ul. Pilsudskiego 14", "Work", new DateTime(1988, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Person", new DateTime(2026, 4, 20, 9, 30, 0, 0, DateTimeKind.Utc), "a.golebiowska@izbacyfrowa.pl", null, "Agnieszka", "Female", "Golebiowska", "[]", new Guid("8a8ecde7-0eaa-4e9e-a67f-3ac9857f4e12"), "509447221", "Koordynator Partnerstw", "Inactive", "[\"events\",\"ngo\"]" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_EmployerId",
                table: "Contacts",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Nip",
                table: "Contacts",
                column: "Nip",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_OrganizationId",
                table: "Contacts",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
