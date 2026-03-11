// ==========================================
// File: IAgentCommissionService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IAgentCommissionService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IAgentCommissionService
    {
        Task<AgentCommissionResponseDto?> GetByIdAsync(int id);
        Task<AgentCommissionResponseDto?> GetByPolicyIdAsync(int policyId);
        Task<IEnumerable<AgentCommissionResponseDto>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<AgentCommissionResponseDto>> GetAllAsync();

        /// <summary>
        /// Backfill: generate commission for an existing policy that has none yet.
        /// Admin-only. Safe to call multiple times (idempotent — skips if already exists).
        /// </summary>
        Task<AgentCommissionResponseDto> GenerateForPolicyAsync(int policyId);
    }
}
