using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyApplicationController : ControllerBase
    {
        private readonly IPolicyApplicationService _service;

        public PolicyApplicationController(
            IPolicyApplicationService service)
        {
            _service = service;
        }


        // ==========================================
        // FLOW 1: FROM REQUEST TO ACTIVE POLICY
        // Step 4: Customer Selects Policy (Creates Application)
        // The customer accepts the agent's suggestion, converting the request into a formal PolicyApplication.
        // ==========================================
        [Authorize(Roles = "Customer")]
        [HttpPost("select")]
        public async Task<IActionResult> SelectPolicy(
            SelectPolicyRequest request)
        {
            // Parses specific logged-in Customer's JWT token ID.
            var customerId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Instructs DB creation of new `PolicyApplication` Entity based upon specific 'SuggestionId' link.
            // Changes initial object Entity Status -> 'PendingApproval'.
            var applicationId =
                await _service.CreateApplicationAsync(
                    request.SuggestionId,
                    customerId);

            // Dispatches back generated Primary Key linking application records directly to customer dashboard GUI elements natively.
            return Ok(new { ApplicationId = applicationId });
        }

        [Authorize(Roles = "Agent")]
        [HttpGet("agent")]
        public async Task<IActionResult> GetByAgent()
        {
            var agentId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var applications = await _service.GetApplicationsByAgentIdAsync(agentId);
            return Ok(applications);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var applications = await _service.GetApplicationsByCustomerIdAsync(customerId);
            return Ok(applications);
        }
    }

    public record SelectPolicyRequest(int SuggestionId);
}
