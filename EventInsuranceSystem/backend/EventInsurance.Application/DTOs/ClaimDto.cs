// ==========================================
// File: ClaimDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for ClaimDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.DTOs
{
    /// <summary>Request DTO for customer raising a claim (Step 12).</summary>
    public class RaiseClaimRequestDto
    {
        public int PolicyId { get; set; }
        public int CustomerId { get; set; }
        public decimal ClaimAmount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>Response DTO returned after a claim is submitted or fetched.</summary>
    public class ClaimResponseDto
    {
        public int Id { get; set; }

        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public decimal ClaimAmount { get; set; }
        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal FraudScore { get; set; }
        public bool FraudFlag { get; set; }

        public DateTime SubmittedDate { get; set; }
        public DateTime? SettlementDate { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
