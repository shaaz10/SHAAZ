// ==========================================
// File: InsuranceRequestRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for InsuranceRequestRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using Microsoft.EntityFrameworkCore;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Domain.Entities;
using EventInsurance.Infrastructure.Persistence;

namespace EventInsurance.Infrastructure.Repositories
{
    public class InsuranceRequestRepository
        : IInsuranceRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public InsuranceRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InsuranceRequest request)
        {
            await _context.InsuranceRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<InsuranceRequest>> GetByAgentIdAsync(int agentId)
        {
            return await _context.InsuranceRequests
                .Where(x => x.AssignedAgentId == agentId)
                .ToListAsync();
        }
        public async Task<InsuranceRequest?> GetByIdAsync(int id)
        {
            return await _context.InsuranceRequests
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<InsuranceRequest>> GetAllAsync()
        {
            return await _context.InsuranceRequests.ToListAsync();
        }

        public async Task<IEnumerable<InsuranceRequest>> GetUnassignedRequestsAsync()
        {
            return await _context.InsuranceRequests
                .Where(x => x.AssignedAgentId == null)
                .ToListAsync();
        }

        public async Task UpdateAsync(InsuranceRequest request)
        {
            _context.InsuranceRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InsuranceRequest>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.InsuranceRequests
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }
    }
}