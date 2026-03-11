// ==========================================
// File: ApplicationDbContext.cs
// Layer: EventInsurance.Infrastructure
// Description: Database Context defining the Entity Framework Core configuration and database sets for the application.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Microsoft.EntityFrameworkCore;

using EventInsurance.Domain.Entities;

namespace EventInsurance.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<InsuranceRequest> InsuranceRequests { get; set; }
        public DbSet<PolicyProduct> PolicyProducts { get; set; }
        public DbSet<PolicySuggestion> PolicySuggestions { get; set; }
        public DbSet<PolicyApplication> PolicyApplications { get; set; }
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<ActivePolicy> ActivePolicies { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AgentCommission> AgentCommissions { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimDocument> ClaimDocuments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        private static string ComputeSha256Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().HasData(
    new Role { Id = 1, RoleName = "Admin" },
    new Role { Id = 2, RoleName = "Customer" },
    new Role { Id = 3, RoleName = "Agent" },
    new Role { Id = 4, RoleName = "ClaimsOfficer" }
);

            // Seed admin user with SHA256 hash
            var adminPasswordHash = ComputeSha256Hash("Admin@123");
            modelBuilder.Entity<User>().HasData(
    new User
    {
        Id = 1,
        FullName = "System Admin",
        Email = "admin@event.com",
        PasswordHash = adminPasswordHash,
        PhoneNumber = "+1234567890",
        RoleId = 1,
        IsActive = true,
        IsLocked = false,
        FailedLoginAttempts = 0,
        CreatedAt = new DateTime(2026, 2, 25, 12, 0, 0, 0, DateTimeKind.Utc),
        UpdatedAt = null
    }
);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ActivePolicy>()
                .HasIndex(p => p.PolicyNumber)
                .IsUnique();

            // InsuranceRequest → Customer
            modelBuilder.Entity<InsuranceRequest>()
                .HasOne(ir => ir.Customer)
                .WithMany()
                .HasForeignKey(ir => ir.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // InsuranceRequest → AssignedAgent
            modelBuilder.Entity<InsuranceRequest>()
                .HasOne(ir => ir.AssignedAgent)
                .WithMany()
                .HasForeignKey(ir => ir.AssignedAgentId)
                .OnDelete(DeleteBehavior.Restrict);
            // PolicyApplication → Customer
            modelBuilder.Entity<PolicyApplication>()
                .HasOne(pa => pa.Customer)
                .WithMany()
                .HasForeignKey(pa => pa.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PolicyApplication → Agent
            modelBuilder.Entity<PolicyApplication>()
                .HasOne(pa => pa.Agent)
                .WithMany()
                .HasForeignKey(pa => pa.AgentId)
                .OnDelete(DeleteBehavior.Restrict);
            // Claim → Customer
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Claim → ClaimsOfficer
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.ClaimsOfficer)
                .WithMany()
                .HasForeignKey(c => c.ClaimsOfficerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentCommission>()
    .HasOne(ac => ac.Agent)
    .WithMany()
    .HasForeignKey(ac => ac.AgentId)
    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PolicySuggestion>()
    .HasOne(ps => ps.SuggestedByAgent)
    .WithMany()
    .HasForeignKey(ps => ps.SuggestedByAgentId)
    .OnDelete(DeleteBehavior.Restrict);
            // ActivePolicy → Customer
            modelBuilder.Entity<ActivePolicy>()
                .HasOne(ap => ap.Customer)
                .WithMany()
                .HasForeignKey(ap => ap.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ActivePolicy → Agent
            modelBuilder.Entity<ActivePolicy>()
                .HasOne(ap => ap.Agent)
                .WithMany()
                .HasForeignKey(ap => ap.AgentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
