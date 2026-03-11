using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventInsurance.API.Controllers
{
    /// <summary>
    /// STEP 13 – AI Fraud Detection
    /// POST /api/fraud/detect/{claimId} → Run fraud scoring on a submitted claim (Admin/ClaimsOfficer)
    /// </summary>
    [ApiController]
    [Route("api/fraud")]
    [Authorize]
    public class FraudDetectionController : ControllerBase
    {
        private readonly IAIFraudDetectionService _fraudService;

        public FraudDetectionController(IAIFraudDetectionService fraudService)
        {
            _fraudService = fraudService;
        }

        // ==========================================
        // FLOW 2: THE CLAIMS LIFECYCLE
        // Step 2: AI Fraud Detection
        // The system analyzes the claim parameters using an AI service to score the likelihood of fraud.
        // ==========================================
        [HttpPost("detect/{claimId}")]
        [Authorize(Roles = "Admin,ClaimsOfficer")]
        public async Task<IActionResult> DetectFraud(int claimId)
        {
            try
            {
                // Calls the fraud detection service to evaluate the claim and update its record
                var result = await _fraudService.DetectFraudAsync(claimId);

                // Returns the calculated fraud score and whether a flag was triggered
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handles cases where the claim might not exist or analysis fails
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
