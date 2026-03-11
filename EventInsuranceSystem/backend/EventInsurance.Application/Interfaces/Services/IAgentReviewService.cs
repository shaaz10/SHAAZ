// ==========================================
// File: IAgentReviewService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IAgentReviewService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IAgentReviewService
    {
        Task<ApplicationReviewResponseDto> ReviewApplicationAsync(
            int applicationId,
            int agentId,
            AgentReviewApplicationDto dto);

        Task<ApplicationReviewResponseDto> EscalateToAdminAsync(
            int applicationId,
            int agentId,
            string notes);
    }
}
