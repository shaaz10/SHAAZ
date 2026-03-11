// ==========================================
// File: IAgentCommissionRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for IAgentCommissionRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Interfaces.Repositories
{
    public interface IAgentCommissionRepository
    {
        Task AddAsync(AgentCommission commission);
        Task<AgentCommission?> GetByIdAsync(int id);
        Task<AgentCommission?> GetByPolicyIdAsync(int policyId);
        Task<IEnumerable<AgentCommission>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<AgentCommission>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
