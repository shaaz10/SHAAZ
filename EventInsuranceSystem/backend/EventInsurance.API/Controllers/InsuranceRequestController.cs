 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsuranceRequestController : ControllerBase
    {
        private readonly IInsuranceRequestService _service;

        public InsuranceRequestController(
            IInsuranceRequestService service)
        {
            _service = service;
        }


        // ==========================================
        // FLOW 1: FROM REQUEST TO ACTIVE POLICY
        // Step 2: Admin Assigns an Agent
        // The Administrator assigns an available agent to the newly created Insurance Request.
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost("assign-agent")]
        public async Task<IActionResult> AssignAgent(AssignAgentDto dto)
        {
            // Administrator delegates tracking. Contacts Service to update entity Database model.
            // Alters 'InsuranceRequest' -> 'AssignedAgentId' property effectively.
            await _service.AssignAgentAsync(dto.RequestId, dto.AgentId);

            return Ok(new { message = "Agent assigned successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("unassigned")]
        public async Task<IActionResult> GetUnassignedRequests()
        {
            // Fetches all InsuranceRequests where 'AssignedAgentId' remains NULL.
            // Populates Admin Dashboard queues for active tracking.
            var requests = await _service.GetUnassignedRequestsAsync();
            return Ok(requests);
        }
        [Authorize(Roles = "Agent")]
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetAssignedRequests()
        {
            // Validates JWT context to extract securely decoded specific remote Agent Identifier.
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (idClaim == null)
                return Unauthorized();

            var agentId = int.Parse(idClaim);

            // Pulls requests specifically tethered directly to logged-in Agent.
            var requests = await _service.GetAssignedRequestsAsync(agentId);

            return Ok(requests);
        }

        // ==========================================
        // FLOW 1: FROM REQUEST TO ACTIVE POLICY
        // Step 1: Customer Requests Insurance 
        // Customer submits event details (type, budget, location, coverage).
        // This hits InsuranceRequestService.CreateRequestAsync to begin the flow.
        // ==========================================
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateInsuranceRequestDto dto)
        {
            try
            {
                // Retrieves customer executing payload directly securely from token header via ClaimTypes
                var customerId = int.Parse(
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier));

                // Injects customer constraints forming core DB request item with constraints parameters applied
                await _service.CreateRequestAsync(customerId, dto);

                return Ok(new { message = "Request submitted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("customer-requests")]
        public async Task<IActionResult> GetCustomerRequests()
        {
            var customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            var requests = await _service.GetCustomerRequestsAsync(customerId);
            return Ok(requests);
        }
    }
}