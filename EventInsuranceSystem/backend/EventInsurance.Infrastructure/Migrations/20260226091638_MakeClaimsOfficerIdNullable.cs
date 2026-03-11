using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventInsurance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeClaimsOfficerIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClaimsOfficerId",
                table: "Claims",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FailedLoginAttempts", "FullName", "IsActive", "IsLocked", "PasswordHash", "PhoneNumber", "RoleId", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2026, 2, 25, 12, 0, 0, 0, DateTimeKind.Utc), "admin@event.com", 0, "System Admin", true, false, "6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc=", "+1234567890", 1, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "ClaimsOfficerId",
                table: "Claims",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 25, 5, 13, 36, 358, DateTimeKind.Utc).AddTicks(1145));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 25, 5, 13, 36, 358, DateTimeKind.Utc).AddTicks(1149));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 25, 5, 13, 36, 358, DateTimeKind.Utc).AddTicks(1149));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 25, 5, 13, 36, 358, DateTimeKind.Utc).AddTicks(1150));
        }
    }
}
