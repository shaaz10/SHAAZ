using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolicySuggestionController : ControllerBase
    {
        private readonly IPolicySuggestionService _service;

        public PolicySuggestionController(
            IPolicySuggestionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Allows an Agent to suggest a specific policy product to a customer request.
        /// Extracts the Agent's ID from the JWT token and associates it with the suggestion.
        /// </summary>
        // ==========================================
        // FLOW 1: FROM REQUEST TO ACTIVE POLICY
        // Step 3: Agent Suggests a Policy
        // The agent reviews the event details and suggests a policy product with a calculated premium.
        // ==========================================
        [Authorize(Roles = "Agent")]
        [HttpPost]
        public async Task<IActionResult> SuggestPolicy(
            SuggestPolicyDto dto)
        {
            // Extract the Agent ID (NameIdentifier) from the authenticated user's claims
            var idClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (idClaim == null)
                return Unauthorized();

            var agentId = int.Parse(idClaim);

            // Pass the suggestion details to the service layer for processing and storage
            await _service.SuggestPolicyAsync(
                agentId,
                dto.RequestId,
                dto.PolicyProductId,
                dto.SuggestedPremium);

            return Ok(new { message = "Policy suggested successfully" });
        }
        /// <summary>
        /// Retrieves all policy suggestions associated with a specific insurance request.
        /// This allows customers to view the options recommended by their assigned agents.
        /// </summary>
        [HttpGet("request/{requestId}")]
        public async Task<IActionResult> GetByRequest(int requestId)
        {
            // Fetches all suggestions linked to the provided requestId from the service
            var result = await
                _service.GetSuggestionsByRequestIdAsync(requestId);

            return Ok(result);
        }
    }
}