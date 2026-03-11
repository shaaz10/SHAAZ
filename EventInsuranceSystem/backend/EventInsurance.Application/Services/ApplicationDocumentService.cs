// ==========================================
// File: ApplicationDocumentService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for ApplicationDocumentService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Services
{
    public class ApplicationDocumentService
        : IApplicationDocumentService
    {
        private readonly IApplicationDocumentRepository _documentRepository;
        private readonly IPolicyApplicationRepository _applicationRepository;

        public ApplicationDocumentService(
            IApplicationDocumentRepository documentRepository,
            IPolicyApplicationRepository applicationRepository)
        {
            _documentRepository = documentRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<int> UploadDocumentAsync(
            int applicationId,
            UploadDocumentDto dto)
        {
            // Verify application exists
            var application = await _applicationRepository
                .GetByIdAsync(applicationId);

            if (application == null)
                throw new Exception("Application not found");

            // Create document
            var document = new ApplicationDocument
            {
                ApplicationId = applicationId,
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                DocumentType = dto.DocumentType,
                ValidationStatus = "Pending",
                ConfidenceScore = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _documentRepository.AddAsync(document);

            return document.Id;
        }

        public async Task<IEnumerable<DocumentResponseDto>> 
            GetApplicationDocumentsAsync(int applicationId)
        {
            var documents = await _documentRepository
                .GetByApplicationIdAsync(applicationId);

            return documents.Select(d => new DocumentResponseDto
            {
                Id = d.Id,
                ApplicationId = d.ApplicationId,
                FileName = d.FileName,
                DocumentType = d.DocumentType,
                ValidationStatus = d.ValidationStatus,
                ConfidenceScore = d.ConfidenceScore
            }).ToList();
        }
    }
}
