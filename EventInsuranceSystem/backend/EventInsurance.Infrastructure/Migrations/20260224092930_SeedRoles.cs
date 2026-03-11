using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventInsurance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "RoleName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 24, 9, 29, 29, 75, DateTimeKind.Utc).AddTicks(5730), "Admin", null },
                    { 2, new DateTime(2026, 2, 24, 9, 29, 29, 75, DateTimeKind.Utc).AddTicks(5733), "Customer", null },
                    { 3, new DateTime(2026, 2, 24, 9, 29, 29, 75, DateTimeKind.Utc).AddTicks(5734), "Agent", null },
                    { 4, new DateTime(2026, 2, 24, 9, 29, 29, 75, DateTimeKind.Utc).AddTicks(5735), "ClaimsOfficer", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
