// ==========================================
// File: IActivePolicyService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IActivePolicyService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IActivePolicyService
    {
        /// <summary>
        /// STEP 9: Create an active policy from an approved application.
        /// Generates a unique PolicyNumber, sets StartDate/EndDate, Status = Active.
        /// </summary>
        Task<ActivePolicyResponseDto> CreateActivePolicyAsync(int applicationId);

        Task<ActivePolicyResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ActivePolicyResponseDto>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<ActivePolicyResponseDto>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<ActivePolicyResponseDto>> GetAllAsync();
    }
}
