// ==========================================
// File: AIDocumentValidationService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for AIDocumentValidationService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Enums;
using System.Net.Http;
using System.Text.Json;

namespace EventInsurance.Application.Services
{
    public class AIDocumentValidationService
        : IAIDocumentValidationService
    {
        private readonly IApplicationDocumentRepository _documentRepository;
        private readonly IPolicyApplicationRepository _applicationRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public AIDocumentValidationService(
            IApplicationDocumentRepository documentRepository,
            IPolicyApplicationRepository applicationRepository,
            IHttpClientFactory httpClientFactory)
        {
            _documentRepository = documentRepository;
            _applicationRepository = applicationRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DocumentValidationResultDto> ValidateAndScoreAsync(
            int applicationId)
        {
            // Get application
            var application = await _applicationRepository
                .GetByIdWithDetailsAsync(applicationId);

            if (application == null)
                throw new Exception("Application not found");

            // Get all documents
            var documents = await _documentRepository
                .GetByApplicationIdAsync(applicationId);

            if (!documents.Any())
                throw new Exception("No documents uploaded for validation");

            var validationResults = new List<DocumentValidationDto>();
            var allConfidenceScores = new List<decimal>();

            using var client = _httpClientFactory.CreateClient();

            foreach (var doc in documents)
            {
                decimal confidenceScore = 0;
                
                using var form = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(doc.FileName ?? "dummy"));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                form.Add(fileContent, "file", doc.FileName ?? "document.txt");

                try
                {
                    var response = await client.PostAsync("http://127.0.0.1:8000/validate-document", form);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var result = JsonSerializer.Deserialize<FastApiValidationResponse>(responseData, options);
                        if (result != null)
                        {
                            confidenceScore = (decimal)result.confidence_score;
                        }
                    }
                    else
                    {
                        // Fallback
                        confidenceScore = SimulateAIValidation(doc.DocumentType);
                    }
                }
                catch
                {
                    // Fallback
                    confidenceScore = SimulateAIValidation(doc.DocumentType);
                }

                var validationStatus = confidenceScore >= 80 ? "Validated" : "RequiresReview";

                // Update document
                doc.ValidationStatus = validationStatus;
                doc.ConfidenceScore = confidenceScore;
                doc.UpdatedAt = DateTime.UtcNow;

                validationResults.Add(new DocumentValidationDto
                {
                    DocumentId = doc.Id,
                    FileName = doc.FileName,
                    ValidationStatus = validationStatus,
                    ConfidenceScore = confidenceScore
                });

                allConfidenceScores.Add(confidenceScore);
            }

            // Calculate overall risk score
            var overallConfidence = allConfidenceScores.Average();
            var riskScore = CalculateRiskScore(overallConfidence, application.Request.EventBudget);
            var riskCategory = DetermineRiskCategory(riskScore);

            // Update application
            application.RiskScore = riskScore;
            application.RiskCategory = riskCategory;
            application.Status = PolicyApplicationStatus.UnderReview;
            application.UpdatedAt = DateTime.UtcNow;

            // Save all changes
            await _documentRepository.SaveChangesAsync();
            await _applicationRepository.SaveChangesAsync();

            return new DocumentValidationResultDto
            {
                ApplicationId = applicationId,
                Documents = validationResults,
                OverallRiskScore = riskScore,
                RiskCategory = riskCategory,
                ValidationSummary = $"Validated {validationResults.Count(d => d.ValidationStatus == "Validated")} of {validationResults.Count} documents"
            };
        }

        /// <summary>
        /// Simulate AI document validation
        /// In production, integrate with actual AI service (like Azure Document Intelligence)
        /// </summary>
        private decimal SimulateAIValidation(string documentType)
        {
            // Simulate confidence score based on document type
            return documentType switch
            {
                "IdentityProof" => 95m,
                "AddressProof" => 92m,
                "InsuranceCertificate" => 98m,
                "EventDeedsAndDetails" => 85m,
                "VenuePermit" => 88m,
                "FinancialProof" => 90m,
                _ => 75m  // Default lower confidence for unknown types
            };
        }

        /// <summary>
        /// Calculate risk score based on confidence and event budget
        /// Lower score = Lower risk
        /// </summary>
        private decimal CalculateRiskScore(decimal confidenceScore, decimal eventBudget)
        {
            // Risk = 100 - confidence (lower confidence = higher risk)
            decimal documentRisk = 100 - confidenceScore;

            // Budget-based risk: Higher budget = Higher risk
            decimal budgetRisk = Math.Min((eventBudget / 100000), 50);  // Cap at 50

            // Weighted average: 60% documents, 40% budget
            decimal totalRisk = (documentRisk * 0.6m) + (budgetRisk * 0.4m);

            return Math.Round(totalRisk, 2);
        }

        /// <summary>
        /// Determine risk category based on risk score
        /// </summary>
        private string DetermineRiskCategory(decimal riskScore)
        {
            return riskScore switch
            {
                <= 15 => "Low",
                <= 35 => "Medium",
                <= 60 => "High",
                _ => "VeryHigh"
            };
        }

        private class FastApiValidationResponse
        {
            public float confidence_score { get; set; }
            public string message { get; set; } = string.Empty;
        }
    }
}
