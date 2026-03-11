// ==========================================
// File: AIFraudDetectionService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for AIFraudDetectionService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using System.Net.Http;
using System.Text.Json;

namespace EventInsurance.Application.Services
{
    /// <summary>
    /// STEP 13 – AI Fraud Detection
    /// UPDATE Claims:
    ///   • FraudScore = from FastAPI (0–100)
    ///   • FraudFlag  = from FastAPI
    /// </summary>
    public class AIFraudDetectionService : IAIFraudDetectionService
    {
        private readonly IClaimRepository _claimRepo;
        private readonly IHttpClientFactory _httpClientFactory;

        public AIFraudDetectionService(IClaimRepository claimRepo, IHttpClientFactory httpClientFactory)
        {
            _claimRepo = claimRepo;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<FraudDetectionResponseDto> DetectFraudAsync(int claimId)
        {
            var claim = await _claimRepo.GetByIdAsync(claimId);
            if (claim == null)
                throw new Exception($"Claim with ID {claimId} not found.");

            decimal fraudScore = 0;
            bool fraudFlag = false;
            string riskLevel = "Low";

            // If there's a document, send it to FastAPI. 
            // Since we are not saving documents locally as per requirement, we simulate 
            // the FastAPI call by sending a dummy file if no real file exists,
            // or we send the first document's bytes if we had them.
            // FastAPI expects a file. We will create a dummy file content describing the claim.
            using var client = _httpClientFactory.CreateClient();
            using var form = new MultipartFormDataContent();
            
            var fileContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(claim.Description ?? "No description"));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
            form.Add(fileContent, "file", $"claim_{claimId}.txt");

            try
            {
                var response = await client.PostAsync("http://127.0.0.1:8000/detect-fraud", form);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<FastApiFraudResponse>(responseData, options);
                    
                    if (result != null)
                    {
                        fraudScore = (decimal)result.fraud_score;
                        fraudFlag = result.fraud_flag;
                        riskLevel = result.risk_level;
                    }
                }
                else
                {
                    // Fallback to simulation if FastAPI is down
                    fraudScore = SimulateFraudScore(claim.ClaimAmount, claim.Description);
                    fraudFlag = fraudScore >= 70;
                    riskLevel = GetRiskLevel(fraudScore);
                }
            }
            catch
            {
                // Fallback to simulation if FastAPI doesn't respond
                fraudScore = SimulateFraudScore(claim.ClaimAmount, claim.Description);
                fraudFlag = fraudScore >= 70;
                riskLevel = GetRiskLevel(fraudScore);
            }

            claim.FraudScore = fraudScore;
            claim.FraudFlag = fraudFlag;
            claim.UpdatedAt = DateTime.UtcNow;
            await _claimRepo.SaveChangesAsync();

            return new FraudDetectionResponseDto
            {
                ClaimId = claimId,
                PolicyNumber = claim.Policy?.PolicyNumber ?? string.Empty,
                ClaimAmount = claim.ClaimAmount,
                FraudScore = fraudScore,
                FraudFlag = fraudFlag,
                RiskLevel = riskLevel,
                Message = fraudFlag
                    ? $"⚠️ Fraud suspected! Score: {fraudScore}/100. Claim is flagged for manual review."
                    : $"✅ Claim appears legitimate. Score: {fraudScore}/100 ({riskLevel} risk)."
            };
        }

        private class FastApiFraudResponse
        {
            public float fraud_score { get; set; }
            public bool fraud_flag { get; set; }
            public string risk_level { get; set; } = string.Empty;
            public string message { get; set; } = string.Empty;
        }

        // ── Simulation Logic (replace with FastAPI call later) ────────────────

        /// <summary>
        /// Simulates a fraud score based on claim amount and keywords.
        /// FastAPI will replace this with a real ML model score.
        /// </summary>
        private static decimal SimulateFraudScore(decimal claimAmount, string description)
        {
            var random = new Random();
            decimal baseScore = random.Next(10, 60);  // Base: 10–60

            // Large claim amounts slightly raise suspicion
            if (claimAmount > 100000) baseScore += 20;
            else if (claimAmount > 50000) baseScore += 10;
            else if (claimAmount > 20000) baseScore += 5;

            // Suspicious keywords in description raise score
            var suspiciousKeywords = new[] { "urgent", "cash", "immediate", "fire", "total loss", "stolen" };
            var desc = description.ToLower();
            foreach (var kw in suspiciousKeywords)
                if (desc.Contains(kw)) baseScore += 8;

            // Cap at 100
            return Math.Min(Math.Round(baseScore, 2), 100);
        }

        private static string GetRiskLevel(decimal score) => score switch
        {
            < 30  => "Low",
            < 50  => "Medium",
            < 70  => "High",
            _     => "Critical"
        };
    }
}
