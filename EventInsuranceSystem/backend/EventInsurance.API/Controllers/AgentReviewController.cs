using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentReviewController : ControllerBase
    {
        private readonly IAgentReviewService _service;

        public AgentReviewController(IAgentReviewService service)
        {
            _service = service;
        }

        /// <summary>
        /// Allows an agent to review a policy application submitted by a customer.
        /// Extracts the Agent's ID from the JWT token and process the review.
        /// </summary>
        // ==========================================
        // FLOW 1: FROM REQUEST TO ACTIVE POLICY
        // Step 6: Agent Review & Admin Approval (Agent Phase)
        // The assigned agent reviews the AI validation score and escalates/approves the application.
        // ==========================================
        [Authorize(Roles = "Agent")]
        [HttpPost("review/{applicationId}")]
        public async Task<IActionResult> ReviewApplication(
            int applicationId,
            [FromBody] AgentReviewApplicationDto dto)
        {
            // Extract the Agent ID (NameIdentifier) from the authenticated user's claims
            var agentId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Pass the review details to the service layer for processing
            var result = await _service.ReviewApplicationAsync(
                applicationId,
                agentId,
                dto);

            // Returns the review result including any updated status
            return Ok(result);
        }

        /// <summary>
        /// Allows an agent to escalate an application to an administrator for further review.
        /// This is used when an application is complex or requires higher-level approval.
        /// </summary>
        [Authorize(Roles = "Agent")]
        [HttpPost("escalate/{applicationId}")]
        public async Task<IActionResult> EscalateToAdmin(
            int applicationId,
            [FromBody] EscalateRequestDto request)
        {
            // Extract the Agent ID (NameIdentifier) from the authenticated user's claims
            var agentId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Calls the service to mark the application for escalation with provided notes
            var result = await _service.EscalateToAdminAsync(
                applicationId,
                agentId,
                request.Notes);

            // Returns the escalation confirmation status
            return Ok(result);
        }
    }

    public record EscalateRequestDto(string Notes);
}
