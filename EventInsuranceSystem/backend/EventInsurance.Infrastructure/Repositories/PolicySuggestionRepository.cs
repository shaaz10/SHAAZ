// ==========================================
// File: PolicySuggestionRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for PolicySuggestionRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Domain.Entities;
using EventInsurance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventInsurance.Infrastructure.Repositories
{
    public class PolicySuggestionRepository
        : IPolicySuggestionRepository
    {
        private readonly ApplicationDbContext _context;

        public PolicySuggestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PolicySuggestion suggestion)
        {
            await _context.PolicySuggestions.AddAsync(suggestion);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<PolicySuggestion>>
    GetByRequestIdAsync(int requestId)
        {
            return await _context.PolicySuggestions
                .Include(ps => ps.PolicyProduct)
                .Where(ps => ps.RequestId == requestId)
                .ToListAsync();
        }
        public async Task<PolicySuggestion?> GetByIdAsync(int id)
        {
            return await _context.PolicySuggestions
                .Include(ps => ps.PolicyProduct)
                .Include(ps => ps.Request)
                .FirstOrDefaultAsync(ps => ps.Id == id);
        }

    }
}
