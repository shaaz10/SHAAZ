// ==========================================
// File: AdminApprovalDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for AdminApprovalDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    public class AdminApprovalRequestDto
    {
        public bool IsApproved { get; set; }
        public string ApprovalNotes { get; set; } = string.Empty;
        public decimal? SuggestedPremium { get; set; }
    }

    public class AdminApprovalResponseDto
    {
        public int ApplicationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string ApprovalNotes { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime ApprovedAt { get; set; }
    }
}
