// ==========================================
// File: ActivePolicyResponseDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for ActivePolicyResponseDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    /// <summary>
    /// Response DTO returned after creating or fetching an Active Policy (Step 9).
    /// </summary>
    public class ActivePolicyResponseDto
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;

        public int ApplicationId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;

        public string PolicyProductName { get; set; } = string.Empty;
        public decimal PremiumAmount { get; set; }
        public decimal CoverageAmount { get; set; }
        public DateTime? EventDate { get; set; }
        public string EventLocation { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
