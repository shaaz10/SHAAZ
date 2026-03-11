// ==========================================
// File: PaymentRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for PaymentRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using Microsoft.EntityFrameworkCore;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Domain.Entities;
using EventInsurance.Infrastructure.Persistence;

namespace EventInsurance.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Policy)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetByPolicyIdAsync(int policyId)
        {
            return await _context.Payments
                .Include(p => p.Policy)
                .Where(p => p.PolicyId == policyId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Policy)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
