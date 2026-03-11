// ==========================================
// File: AgentCommissionService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for AgentCommissionService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Services
{
    /// <summary>
    /// STEP 10 – Commission Generation (read/view side).
    /// The commission INSERT itself happens inside ActivePolicyService.CreateActivePolicyAsync.
    /// This service provides query access to commission records.
    /// </summary>
    public class AgentCommissionService : IAgentCommissionService
    {
        private readonly IAgentCommissionRepository _commissionRepo;
        private readonly IActivePolicyRepository _activePolicyRepo;

        private const decimal CommissionRate = 0.10m;

        public AgentCommissionService(
            IAgentCommissionRepository commissionRepo,
            IActivePolicyRepository activePolicyRepo)
        {
            _commissionRepo = commissionRepo;
            _activePolicyRepo = activePolicyRepo;
        }

        public async Task<AgentCommissionResponseDto?> GetByIdAsync(int id)
        {
            var c = await _commissionRepo.GetByIdAsync(id);
            return c == null ? null : MapToDto(c);
        }

        public async Task<AgentCommissionResponseDto?> GetByPolicyIdAsync(int policyId)
        {
            var c = await _commissionRepo.GetByPolicyIdAsync(policyId);
            return c == null ? null : MapToDto(c);
        }

        public async Task<IEnumerable<AgentCommissionResponseDto>> GetByAgentIdAsync(int agentId)
        {
            var list = await _commissionRepo.GetByAgentIdAsync(agentId);
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<AgentCommissionResponseDto>> GetAllAsync()
        {
            var list = await _commissionRepo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<AgentCommissionResponseDto> GenerateForPolicyAsync(int policyId)
        {
            // 1. Check if commission already exists (idempotent)
            var existing = await _commissionRepo.GetByPolicyIdAsync(policyId);
            if (existing != null)
                throw new Exception(
                    $"Commission already exists for policy {policyId}. " +
                    $"Amount: {existing.CommissionAmount:C}, IsPaid: {existing.IsPaid}");

            // 2. Load the active policy to get AgentId and PremiumAmount
            var policy = await _activePolicyRepo.GetByIdAsync(policyId);
            if (policy == null)
                throw new Exception($"Active policy with ID {policyId} not found.");

            // 3. Calculate commission (10% of premium)
            var commissionAmount = Math.Round(policy.PremiumAmount * CommissionRate, 2);

            // 4. Create and save commission
            var commission = new AgentCommission
            {
                AgentId = policy.AgentId,
                PolicyId = policy.Id,
                CommissionAmount = commissionAmount,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow
            };
            await _commissionRepo.AddAsync(commission);

            // 5. Reload with navigation properties for the response
            var saved = await _commissionRepo.GetByPolicyIdAsync(policyId);
            return MapToDto(saved!);
        }

        private static AgentCommissionResponseDto MapToDto(AgentCommission c)
        {
            // Commission percentage back-calculated from amount vs policy premium
            var policyPremium = c.Policy?.PremiumAmount ?? 0;
            var commissionPct = policyPremium > 0
                ? Math.Round((c.CommissionAmount / policyPremium) * 100, 2)
                : 0;

            return new AgentCommissionResponseDto
            {
                Id = c.Id,
                AgentId = c.AgentId,
                AgentName = c.Agent?.FullName ?? string.Empty,
                PolicyId = c.PolicyId,
                PolicyNumber = c.Policy?.PolicyNumber ?? string.Empty,
                CommissionAmount = c.CommissionAmount,
                CommissionPercentage = commissionPct,
                IsPaid = c.IsPaid,
                PaidDate = c.PaidDate,
                CreatedAt = c.CreatedAt,
                Message = c.IsPaid
                    ? $"Commission paid on {c.PaidDate:yyyy-MM-dd}"
                    : "Commission pending payment"
            };
        }
    }
}
