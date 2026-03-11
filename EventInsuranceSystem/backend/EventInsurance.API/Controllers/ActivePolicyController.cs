using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventInsurance.API.Controllers
{
    /// <summary>
    /// STEP 9 – Active Policy Creation
    /// POST /api/activepolicies/create/{applicationId}  → Admin creates active policy after approval
    /// GET  /api/activepolicies/{id}                   → Get policy by ID
    /// GET  /api/activepolicies/customer/{customerId}  → Customer's policies
    /// GET  /api/activepolicies/agent/{agentId}        → Agent's policies
    /// GET  /api/activepolicies                        → All policies (Admin)
    /// </summary>
    [ApiController]
    [Route("api/activepolicies")]
    [Authorize]
    public class ActivePolicyController : ControllerBase
    {
        private readonly IActivePolicyService _activePolicyService;

        public ActivePolicyController(IActivePolicyService activePolicyService)
        {
            _activePolicyService = activePolicyService;
        }

        // ==========================================
        // FLOW 1: FROM REQUEST TO ACTIVE POLICY
        // Step 7: Active Policy & Commission Generation
        // Once approved, the application is converted into an Active Policy. An Agent Commission (e.g., 10%) is automatically calculated and recorded.
        // ==========================================
        [HttpPost("create/{applicationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateActivePolicy(int applicationId)
        {
            // Triggers backend conversion processes transferring 'Approved' PolicyApplication elements into newly minted 'ActivePolicy'.
            // Establishes critical Coverage validity timestamps mapping to Database parameters dynamically automatically inside CreateActivePolicyAsync logic blocks.
            try
            {
                var result = await _activePolicyService.CreateActivePolicyAsync(applicationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific active policy by its ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Specifically queries Data layers extracting single record associated explicitly mapped to primary ActivePolicy Identifier.
            var result = await _activePolicyService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Active policy with ID {id} not found." });
            return Ok(result);
        }

        /// <summary>
        /// Get all active policies for a customer.
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var results = await _activePolicyService.GetByCustomerIdAsync(customerId);
            return Ok(results);
        }

        /// <summary>
        /// Get all active policies assigned to an agent.
        /// </summary>
        [HttpGet("agent/{agentId}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetByAgent(int agentId)
        {
            var results = await _activePolicyService.GetByAgentIdAsync(agentId);
            return Ok(results);
        }

        /// <summary>
        /// Get all active policies (Admin only).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var results = await _activePolicyService.GetAllAsync();
            return Ok(results);
        }
    }
}
