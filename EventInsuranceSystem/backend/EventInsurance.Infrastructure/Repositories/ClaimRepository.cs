// ==========================================
// File: ClaimRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for ClaimRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using Microsoft.EntityFrameworkCore;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Domain.Entities;
using EventInsurance.Domain.Enums;
using EventInsurance.Infrastructure.Persistence;

namespace EventInsurance.Infrastructure.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly ApplicationDbContext _context;

        public ClaimRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Claim claim)
        {
            await _context.Claims.AddAsync(claim);
            await _context.SaveChangesAsync();
        }

        public async Task<Claim?> GetByIdAsync(int id)
        {
            return await _context.Claims
                .Include(c => c.Policy)
                .Include(c => c.Customer)
                .Include(c => c.ClaimsOfficer)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Claim>> GetByPolicyIdAsync(int policyId)
        {
            return await _context.Claims
                .Include(c => c.Policy)
                .Include(c => c.Customer)
                .Where(c => c.PolicyId == policyId)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Claims
                .Include(c => c.Policy)
                .Include(c => c.Customer)
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetByOfficerIdAsync(int officerId)
        {
            return await _context.Claims
                .Include(c => c.Policy)
                .Include(c => c.Customer)
                .Include(c => c.ClaimsOfficer)
                .Where(c => c.ClaimsOfficerId == officerId)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetByStatusAsync(ClaimStatus status)
        {
            return await _context.Claims
                .Include(c => c.Policy)
                .Include(c => c.Customer)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _context.Claims
                .Include(c => c.Policy)
                .Include(c => c.Customer)
                .Include(c => c.ClaimsOfficer)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
