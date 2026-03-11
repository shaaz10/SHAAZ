// ==========================================
// File: IApplicationDocumentService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IApplicationDocumentService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IApplicationDocumentService
    {
        Task<int> UploadDocumentAsync(
            int applicationId,
            UploadDocumentDto dto);

        Task<IEnumerable<DocumentResponseDto>> 
            GetApplicationDocumentsAsync(int applicationId);
    }
}
