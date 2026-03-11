using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventInsurance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 3, 7, 48, 7, 717, DateTimeKind.Utc).AddTicks(6696));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 3, 7, 48, 7, 717, DateTimeKind.Utc).AddTicks(6699));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 3, 7, 48, 7, 717, DateTimeKind.Utc).AddTicks(6700));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 3, 7, 48, 7, 717, DateTimeKind.Utc).AddTicks(6701));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
