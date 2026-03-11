// ==========================================
// File: IAIFraudDetectionService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IAIFraudDetectionService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IAIFraudDetectionService
    {
        /// <summary>
        /// STEP 13: Run AI fraud detection on a submitted claim.
        /// Updates Claims.FraudScore and Claims.FraudFlag.
        /// Currently uses simulated random scoring.
        /// TODO: Replace SimulateFraudScore() with real FastAPI call when ready.
        /// </summary>
        Task<FraudDetectionResponseDto> DetectFraudAsync(int claimId);
    }
}
