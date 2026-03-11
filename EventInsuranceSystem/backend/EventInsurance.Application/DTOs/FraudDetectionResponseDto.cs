// ==========================================
// File: FraudDetectionResponseDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for FraudDetectionResponseDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    /// <summary>Response DTO after AI fraud detection runs on a claim (Step 13).</summary>
    public class FraudDetectionResponseDto
    {
        public int ClaimId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal ClaimAmount { get; set; }

        public decimal FraudScore { get; set; }        // 0–100 (higher = more suspicious)
        public bool FraudFlag { get; set; }            // true if FraudScore >= 70
        public string RiskLevel { get; set; } = string.Empty; // Low / Medium / High / Critical

        public string Message { get; set; } = string.Empty;
    }
}
