// ==========================================
// File: PolicyApplicationRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for PolicyApplicationRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventInsurance.Domain.Entities;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Infrastructure.Persistence;

namespace EventInsurance.Infrastructure.Repositories
{
    public class PolicyApplicationRepository
        : IPolicyApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public PolicyApplicationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PolicyApplication application)
        {
            await _context.PolicyApplications
                .AddAsync(application);

            await _context.SaveChangesAsync();
        }

        public async Task<PolicyApplication?> GetByIdAsync(int id)
        {
            return await _context.PolicyApplications
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PolicyApplication?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.PolicyApplications
                .Include(p => p.Customer)
                .Include(p => p.Agent)
                .Include(p => p.PolicyProduct)
                .Include(p => p.Request)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PolicyApplication>>
            GetByCustomerIdAsync(int customerId)
        {
            return await _context.PolicyApplications
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PolicyApplication>> GetByAgentIdAsync(int agentId)
        {
            return await _context.PolicyApplications
                .Where(p => p.AgentId == agentId)
                .Include(p => p.Customer)
                .Include(p => p.PolicyProduct)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PolicyApplication>> GetEscalatedAsync()
        {
            return await _context.PolicyApplications
                .Where(p => p.Status == Domain.Enums.PolicyApplicationStatus.EscalatedToAdmin)
                .Include(p => p.Customer)
                .Include(p => p.Agent)
                .Include(p => p.PolicyProduct)
                .Include(p => p.Request)
                .ToListAsync();
        }

        public async Task<IEnumerable<PolicyApplication>> GetApprovedAsync()
        {
            var activePolicyAppIds = await _context.ActivePolicies.Select(ap => ap.ApplicationId).ToListAsync();

            return await _context.PolicyApplications
                .Where(p => p.Status == Domain.Enums.PolicyApplicationStatus.Approved && !activePolicyAppIds.Contains(p.Id))
                .Include(p => p.Customer)
                .Include(p => p.Agent)
                .Include(p => p.PolicyProduct)
                .Include(p => p.Request)
                .ToListAsync();
        }
    }
}