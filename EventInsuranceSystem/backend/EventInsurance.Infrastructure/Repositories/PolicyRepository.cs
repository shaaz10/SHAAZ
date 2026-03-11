// ==========================================
// File: PolicyRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for PolicyRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using Microsoft.EntityFrameworkCore;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Domain.Entities;
using EventInsurance.Infrastructure.Persistence;

namespace EventInsurance.Infrastructure.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly ApplicationDbContext _context;

        public PolicyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PolicyProduct> GetProductByIdAsync(int id)
        {
            return await _context.PolicyProducts.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PolicyProduct>> GetAllProductsAsync()
        {
            return await _context.PolicyProducts.Where(p => p.IsActive).ToListAsync();
        }

        public async Task AddProductAsync(PolicyProduct product)
        {
            await _context.PolicyProducts.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<PolicySuggestion> GetSuggestionByIdAsync(int id)
        {
            return await _context.PolicySuggestions
                .Include(s => s.PolicyProduct)
                .Include(s => s.Request)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddSuggestionAsync(PolicySuggestion suggestion)
        {
            await _context.PolicySuggestions.AddAsync(suggestion);
            await _context.SaveChangesAsync();
        }

        public async Task<PolicyApplication> GetApplicationByIdAsync(int id)
        {
            return await _context.PolicyApplications
                .Include(a => a.Customer)
                .Include(a => a.Agent)
                .Include(a => a.PolicyProduct)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddApplicationAsync(PolicyApplication application)
        {
            await _context.PolicyApplications.AddAsync(application);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateApplicationAsync(PolicyApplication application)
        {
            _context.PolicyApplications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task AddActivePolicyAsync(ActivePolicy policy)
        {
            await _context.ActivePolicies.AddAsync(policy);
            await _context.SaveChangesAsync();
        }
    }
}
