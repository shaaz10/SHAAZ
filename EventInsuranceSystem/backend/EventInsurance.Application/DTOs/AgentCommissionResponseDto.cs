// ==========================================
// File: AgentCommissionResponseDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for AgentCommissionResponseDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    /// <summary>
    /// Response DTO for AgentCommission records (Step 10).
    /// </summary>
    public class AgentCommissionResponseDto
    {
        public int Id { get; set; }

        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;

        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;

        public decimal CommissionAmount { get; set; }
        public decimal CommissionPercentage { get; set; }

        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
