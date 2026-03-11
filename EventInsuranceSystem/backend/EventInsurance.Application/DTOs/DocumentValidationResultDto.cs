// ==========================================
// File: DocumentValidationResultDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for DocumentValidationResultDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    public class DocumentValidationResultDto
    {
        public int ApplicationId { get; set; }
        public List<DocumentValidationDto> Documents { get; set; } 
            = new();
        public decimal OverallRiskScore { get; set; }
        public string RiskCategory { get; set; } = string.Empty;
        public string ValidationSummary { get; set; } = string.Empty;
    }

    public class DocumentValidationDto
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ValidationStatus { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
    }
}
