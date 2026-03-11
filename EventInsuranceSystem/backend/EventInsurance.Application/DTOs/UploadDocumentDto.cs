// ==========================================
// File: UploadDocumentDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for UploadDocumentDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    public class UploadDocumentDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
    }

    public class DocumentResponseDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string FileName { get; set; }
        public string DocumentType { get; set; }
        public string ValidationStatus { get; set; }
        public decimal ConfidenceScore { get; set; }
    }
}
