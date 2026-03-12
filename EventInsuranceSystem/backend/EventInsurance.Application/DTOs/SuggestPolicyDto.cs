// ==========================================
// File: SuggestPolicyDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for SuggestPolicyDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    public class SuggestPolicyDto
    {
        public int RequestId { get; set; }
        public int PolicyProductId { get; set; }
        public decimal SuggestedPremium { get; set; }
        public decimal? CustomCoverageAmount { get; set; }
    }
}