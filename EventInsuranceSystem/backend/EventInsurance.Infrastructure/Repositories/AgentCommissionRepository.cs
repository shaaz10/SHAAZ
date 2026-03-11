// ==========================================
// File: AgentCommissionRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for AgentCommissionRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using Microsoft.EntityFrameworkCore;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Domain.Entities;
using EventInsurance.Infrastructure.Persistence;

namespace EventInsurance.Infrastructure.Repositories
{
    public class AgentCommissionRepository : IAgentCommissionRepository
    {
        private readonly ApplicationDbContext _context;

        public AgentCommissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AgentCommission commission)
        {
            await _context.AgentCommissions.AddAsync(commission);
            await _context.SaveChangesAsync();
        }

        public async Task<AgentCommission?> GetByIdAsync(int id)
        {
            return await _context.AgentCommissions
                .Include(c => c.Agent)
                .Include(c => c.Policy)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<AgentCommission?> GetByPolicyIdAsync(int policyId)
        {
            return await _context.AgentCommissions
                .Include(c => c.Agent)
                .Include(c => c.Policy)
                .FirstOrDefaultAsync(c => c.PolicyId == policyId);
        }

        public async Task<IEnumerable<AgentCommission>> GetByAgentIdAsync(int agentId)
        {
            return await _context.AgentCommissions
                .Include(c => c.Agent)
                .Include(c => c.Policy)
                .Where(c => c.AgentId == agentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AgentCommission>> GetAllAsync()
        {
            return await _context.AgentCommissions
                .Include(c => c.Agent)
                .Include(c => c.Policy)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
