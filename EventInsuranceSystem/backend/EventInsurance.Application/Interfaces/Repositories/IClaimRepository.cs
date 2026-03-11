// ==========================================
// File: IClaimRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for IClaimRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Domain.Entities;
using EventInsurance.Domain.Enums;

namespace EventInsurance.Application.Interfaces.Repositories
{
    public interface IClaimRepository
    {
        Task AddAsync(Claim claim);
        Task<Claim?> GetByIdAsync(int id);
        Task<IEnumerable<Claim>> GetByPolicyIdAsync(int policyId);
        Task<IEnumerable<Claim>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Claim>> GetByOfficerIdAsync(int officerId);
        Task<IEnumerable<Claim>> GetByStatusAsync(ClaimStatus status);
        Task<IEnumerable<Claim>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
