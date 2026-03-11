// ==========================================
// File: IPaymentRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for IPaymentRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<Payment?> GetByIdAsync(int id);
        Task<IEnumerable<Payment>> GetByPolicyIdAsync(int policyId);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
