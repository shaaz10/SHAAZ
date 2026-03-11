using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventInsurance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPremiumAndCoverageToPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CoverageAmount",
                table: "PolicyApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PremiumAmount",
                table: "PolicyApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CoverageAmount",
                table: "ActivePolicies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 2, 9, 35, 47, 626, DateTimeKind.Utc).AddTicks(49));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 2, 9, 35, 47, 626, DateTimeKind.Utc).AddTicks(52));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 2, 9, 35, 47, 626, DateTimeKind.Utc).AddTicks(53));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 2, 9, 35, 47, 626, DateTimeKind.Utc).AddTicks(54));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverageAmount",
                table: "PolicyApplications");

            migrationBuilder.DropColumn(
                name: "PremiumAmount",
                table: "PolicyApplications");

            migrationBuilder.DropColumn(
                name: "CoverageAmount",
                table: "ActivePolicies");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 9, 16, 36, 996, DateTimeKind.Utc).AddTicks(7511));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 9, 16, 36, 996, DateTimeKind.Utc).AddTicks(7514));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 9, 16, 36, 996, DateTimeKind.Utc).AddTicks(7515));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 9, 16, 36, 996, DateTimeKind.Utc).AddTicks(7516));
        }
    }
}
