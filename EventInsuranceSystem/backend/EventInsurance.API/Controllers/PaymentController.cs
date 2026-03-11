using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventInsurance.Application.DTOs;

namespace EventInsurance.API.Controllers
{
    /// <summary>
    /// STEP 11 – Customer Makes Payment
    /// POST /api/payments              → Customer makes a payment against a policy
    /// GET  /api/payments/{id}         → Get payment by ID
    /// GET  /api/payments/policy/{id}  → All payments for a policy
    /// GET  /api/payments              → All payments (Admin)
    /// </summary>
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// STEP 11: Customer makes a payment against their active policy.
        /// Body: { "policyId": 1, "amount": 5000.00, "transactionReference": "optional" }
        /// </summary>
        /// <summary>
        /// Processes a new payment record for an active policy.
        /// Records the transaction details and links them to the specified policy ID.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> MakePayment([FromBody] MakePaymentRequestDto dto)
        {
            try
            {
                // Calls the payment service to record the transaction in the database
                var result = await _paymentService.MakePaymentAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Returns an error message if the payment cannot be processed (e.g. invalid policy or amount)
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a single payment record by its unique ID.
        /// Allows customers and admins to verify specific transaction details.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            // Fetches the payment record from the service and returns 404 if not found
            var result = await _paymentService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Payment {id} not found." });
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all payments associated with a specific active policy.
        /// Helps in tracking the payment history for an insurance coverage.
        /// </summary>
        [HttpGet("policy/{policyId}")]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<IActionResult> GetByPolicy(int policyId)
        {
            // Queries the database for all payment records linked to the policyId
            var results = await _paymentService.GetByPolicyIdAsync(policyId);
            return Ok(results);
        }

        /// <summary>
        /// Retrieves every payment record in the system.
        /// Restricted to Admins for financial reporting and broad oversight.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            // Fetches all payments from the database via the service layer
            var results = await _paymentService.GetAllAsync();
            return Ok(results);
        }
    }
}
