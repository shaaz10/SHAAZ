using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventInsurance.API.Controllers
{
    /// <summary>
    /// STEP 10 – Commission Records
    /// GET /api/commissions                    → All commissions (Admin)
    /// GET /api/commissions/{id}               → Single commission
    /// GET /api/commissions/policy/{policyId}  → Commission for a specific policy
    /// GET /api/commissions/agent/{agentId}    → All commissions for an agent
    /// </summary>
    [ApiController]
    [Route("api/commissions")]
    [Authorize]
    public class AgentCommissionController : ControllerBase
    {
        private readonly IAgentCommissionService _commissionService;

        public AgentCommissionController(IAgentCommissionService commissionService)
        {
            _commissionService = commissionService;
        }

        /// <summary>
        /// Retrieves all commission records from the database.
        /// This is an Admin-only function used for auditing all agent earnings.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            // Calls the commission service to fetch all records and returns them as a list
            var results = await _commissionService.GetAllAsync();
            return Ok(results);
        }

        /// <summary>
        /// Retrieves a single commission record by its unique ID.
        /// Useful for viewing detailed breakdown of a specific payment.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetById(int id)
        {
            // Fetches the specific record from the service and returns 404 if it doesn't exist
            var result = await _commissionService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Commission with ID {id} not found." });
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the commission associated with a specific active policy.
        /// This helps trace how much commission was generated for a particular event insurance policy.
        /// </summary>
        [HttpGet("policy/{policyId}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetByPolicy(int policyId)
        {
            // Queries the database for a commission record linked to the provided policy ID
            var result = await _commissionService.GetByPolicyIdAsync(policyId);
            if (result == null)
                return NotFound(new { message = $"No commission found for policy {policyId}." });
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all commissions earned by a specific agent.
        /// Filters the global list to only show earnings for the requested agent.
        /// </summary>
        [HttpGet("agent/{agentId}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetByAgent(int agentId)
        {
            // Calls the service to filter commissions by agent ID
            var results = await _commissionService.GetByAgentIdAsync(agentId);
            return Ok(results);
        }

        /// <summary>
        /// Manually triggers commission generation for a specific policy.
        /// This is typically used for data recovery or backfilling missing records.
        /// </summary>
        [HttpPost("generate/{policyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateForPolicy(int policyId)
        {
            try
            {
                // Instructs the service to calculate and save a new commission for the policy
                var result = await _commissionService.GenerateForPolicyAsync(policyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handles errors such as duplicates or invalid policy IDs
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
