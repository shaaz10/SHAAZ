// ==========================================
// File: IAIDocumentValidationService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IAIDocumentValidationService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IAIDocumentValidationService
    {
        /// <summary>
        /// Validates documents and performs risk assessment
        /// </summary>
        Task<DocumentValidationResultDto> ValidateAndScoreAsync(
            int applicationId);
    }
}
