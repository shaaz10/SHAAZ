// ==========================================
// File: ActivePolicyRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for ActivePolicyRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using Microsoft.EntityFrameworkCore;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Domain.Entities;
using EventInsurance.Infrastructure.Persistence;

namespace EventInsurance.Infrastructure.Repositories
{
    public class ActivePolicyRepository : IActivePolicyRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivePolicyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ActivePolicy policy)
        {
            await _context.ActivePolicies.AddAsync(policy);
            await _context.SaveChangesAsync();
        }

        public async Task<ActivePolicy?> GetByIdAsync(int id)
        {
            return await _context.ActivePolicies
                .Include(p => p.Customer)
                .Include(p => p.Agent)
                .Include(p => p.Application)
                    .ThenInclude(a => a.PolicyProduct)
                .Include(p => p.Application)
                    .ThenInclude(a => a.Request)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ActivePolicy?> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.ActivePolicies
                .Include(p => p.Customer)
                .Include(p => p.Agent)
                .Include(p => p.Application)
                    .ThenInclude(a => a.PolicyProduct)
                .Include(p => p.Application)
                    .ThenInclude(a => a.Request)
                .FirstOrDefaultAsync(p => p.ApplicationId == applicationId);
        }

        public async Task<IEnumerable<ActivePolicy>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.ActivePolicies
                .Include(p => p.Customer)
                .Include(p => p.Agent)
                .Include(p => p.Application)
                    .ThenInclude(a => a.PolicyProduct)
                .Include(p => p.Application)
                    .ThenInclude(a => a.Request)
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivePolicy>> GetByAgentIdAsync(int agentId)
        {
            return await _context.ActivePolicies
                .Include(p => p.Customer)
                .Include(p => p.Agent)
                .Include(p => p.Application)
                    .ThenInclude(a => a.PolicyProduct)
                .Include(p => p.Application)
                    .ThenInclude(a => a.Request)
                .Where(p => p.AgentId == agentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivePolicy>> GetAllAsync()
        {
            return await _context.ActivePolicies
                .Include(p => p.Customer)
                .Include(p => p.Agent)
                .Include(p => p.Application)
                    .ThenInclude(a => a.PolicyProduct)
                .Include(p => p.Application)
                    .ThenInclude(a => a.Request)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
