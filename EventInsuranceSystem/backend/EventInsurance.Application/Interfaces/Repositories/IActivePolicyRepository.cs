// ==========================================
// File: IActivePolicyRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for IActivePolicyRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Interfaces.Repositories
{
    public interface IActivePolicyRepository
    {
        Task<ActivePolicy?> GetByIdAsync(int id);
        Task<ActivePolicy?> GetByApplicationIdAsync(int applicationId);
        Task<IEnumerable<ActivePolicy>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<ActivePolicy>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<ActivePolicy>> GetAllAsync();
        Task AddAsync(ActivePolicy policy);
        Task UpdateAsync(ActivePolicy policy);
        Task SaveChangesAsync();
    }
}
