using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIValidationController : ControllerBase
    {
        private readonly IAIDocumentValidationService _service;

        public AIValidationController(
            IAIDocumentValidationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Triggers AI-based document validation for a specific policy application.
        /// Analyzes uploaded documents to ensure they meet the policy requirements.
        /// </summary>
        [Authorize(Roles = "Admin,Agent")]
        [HttpPost("validate/{applicationId}")]
        public async Task<IActionResult> ValidateDocuments(
            int applicationId)
        {
            // Calls the AI service to validate documents and calculate a confidence score
            var result = await _service
                .ValidateAndScoreAsync(applicationId);

            // Returns the AI analysis results including validation status and scores
            return Ok(result);
        }
    }
}
