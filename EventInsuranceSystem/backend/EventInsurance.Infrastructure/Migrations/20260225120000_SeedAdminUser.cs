using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Security.Cryptography;
using System.Text;

#nullable disable

namespace EventInsurance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Admin password: Admin@123 (hashed with SHA256)
            var adminPasswordHash = HashPassword("Admin@123");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "FullName", "Email", "PasswordHash", "PhoneNumber", "RoleId", "IsActive", "IsLocked", "FailedLoginAttempts", "CreatedAt", "UpdatedAt" },
                values: new object[] 
                {
                    "System Admin",
                    "admin@event.com",
                    adminPasswordHash,
                    "+1234567890",
                    1, // Admin role
                    true,
                    false,
                    0,
                    new DateTime(2026, 2, 25, 12, 0, 0, 0, DateTimeKind.Utc),
                    null
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Email",
                keyValue: "admin@event.com");
        }
    }
}
