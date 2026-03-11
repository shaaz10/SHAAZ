// ==========================================
// File: ClaimDocumentDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for ClaimDocumentDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    /// <summary>Request DTO for uploading a claim document.</summary>
    public class UploadClaimDocumentDto
    {
        public int ClaimId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;   // Simulated path / URL
    }

    /// <summary>Response DTO for a claim document.</summary>
    public class ClaimDocumentResponseDto
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>Request DTO for Claims Officer to approve/reject a claim (Step 14).</summary>
    public class ClaimReviewDto
    {
        public int ClaimsOfficerId { get; set; }
        public string Notes { get; set; } = string.Empty;  // Optional review notes
    }

    /// <summary>Request DTO for settling a claim (Step 15).</summary>
    public class ClaimSettlementDto
    {
        public int ClaimsOfficerId { get; set; }
        public string SettlementNotes { get; set; } = string.Empty;
    }
}
