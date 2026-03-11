// ==========================================
// File: AgentReviewApplicationDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for AgentReviewApplicationDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    public class AgentReviewApplicationDto
    {
        public string ReviewNotes { get; set; } = string.Empty;
        public bool EscalateToAdmin { get; set; } = false;
        public bool IsApproved { get; set; } = false;
    }

    public class ApplicationReviewResponseDto
    {
        public int ApplicationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ReviewNotes { get; set; } = string.Empty;
        public bool IsEscalated { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
