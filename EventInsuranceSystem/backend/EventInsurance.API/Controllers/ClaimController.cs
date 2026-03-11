using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventInsurance.API.Controllers
{
    /// <summary>
    /// STEP 12 – Customer Raises a Claim
    /// POST /api/claims                        → Customer submits a claim
    /// GET  /api/claims                        → All claims (Admin/ClaimsOfficer)
    /// GET  /api/claims/{id}                   → Get specific claim
    /// GET  /api/claims/policy/{policyId}      → Claims for a policy
    /// GET  /api/claims/customer/{customerId}  → Customer's own claims
    /// GET  /api/claims/status/{status}        → Filter by status (Admin)
    /// </summary>
    [ApiController]
    [Route("api/claims")]
    [Authorize]
    public class ClaimController : ControllerBase
    {
        private readonly IClaimService _claimService;

        public ClaimController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        /// <summary>
        /// STEP 12: Customer raises a claim against their active policy.
        /// Body: { "policyId": 1, "customerId": 2, "claimAmount": 15000, "description": "..." }
        /// </summary>
        // ==========================================
        // FLOW 2: THE CLAIMS LIFECYCLE
        // Step 1: Customer Raises a Claim
        // Customer selects an Active Policy, enters a claim amount, and provides a description of the incident.
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> RaiseClaim([FromBody] RaiseClaimRequestDto dto)
        {
            // First entry point for Claim submissions. Matches ActivePolicy existence implicitly.
            // Transacts creation of 'Claim' SQL object under status -> 'Submitted'
            try
            {
                var result = await _claimService.RaiseClaimAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Get all claims (Admin and ClaimsOfficer only).</summary>
        [HttpGet]
        [Authorize(Roles = "Admin,ClaimsOfficer")]
        public async Task<IActionResult> GetAll()
        {
            var results = await _claimService.GetAllAsync();
            return Ok(results);
        }

        /// <summary>Get a specific claim by ID.</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,ClaimsOfficer,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _claimService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Claim {id} not found." });
            return Ok(result);
        }

        /// <summary>Get all claims for a specific policy.</summary>
        [HttpGet("policy/{policyId}")]
        [Authorize(Roles = "Admin,ClaimsOfficer,Customer")]
        public async Task<IActionResult> GetByPolicy(int policyId)
        {
            var results = await _claimService.GetByPolicyIdAsync(policyId);
            return Ok(results);
        }

        /// <summary>Get all claims submitted by a customer.</summary>
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Admin,ClaimsOfficer,Customer")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var results = await _claimService.GetByCustomerIdAsync(customerId);
            return Ok(results);
        }

        /// <summary>
        /// Filter claims by status.
        /// Status values: Submitted, UnderReview, Approved, Rejected, Settled
        /// </summary>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,ClaimsOfficer")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            if (!Enum.TryParse<ClaimStatus>(status, true, out var claimStatus))
                return BadRequest(new
                {
                    message = $"Invalid status '{status}'.",
                    validValues = Enum.GetNames(typeof(ClaimStatus))
                });

            var results = await _claimService.GetByStatusAsync(claimStatus);
            return Ok(results);
        }

        // ── ADMIN: Assign Claims Officer ──────────────────────────────────────

        /// <summary>
        /// ADMIN assigns a Claims Officer to a claim.
        /// Claim moves to 'UnderReview' status automatically.
        /// Body: { "claimsOfficerId": 5 }
        /// </summary>
        // ==========================================
        // FLOW 2: THE CLAIMS LIFECYCLE
        // Step 3: Admin Assigns a Claims Officer
        // Admin views the unassigned claim (and its fraud score) and assigns a dedicated Claims Officer.
        // ==========================================
        [HttpPost("{claimId}/assign-officer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignOfficer(int claimId, [FromBody] int claimsOfficerId)
        {
            try
            {
                var result = await _claimService.AssignOfficerAsync(claimId, claimsOfficerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Claims Officer sees all claims assigned to them.</summary>
        [HttpGet("officer/{officerId}")]
        [Authorize(Roles = "Admin,ClaimsOfficer")]
        public async Task<IActionResult> GetByOfficer(int officerId)
        {
            var results = await _claimService.GetByOfficerIdAsync(officerId);
            return Ok(results);
        }

        // ── STEP 14: Claims Officer Review ────────────────────────────────────

        /// <summary>
        /// STEP 14a: Claims Officer approves a claim.
        /// Body: { "claimsOfficerId": 5, "notes": "Documents verified, claim is valid" }
        /// </summary>
        // ==========================================
        // FLOW 2: THE CLAIMS LIFECYCLE
        // Step 4: Officer Investigation (Approve/Reject)
        // The Claims Officer reviews the claim and uploaded evidence, then approves or rejects it.
        // ==========================================
        [HttpPost("{claimId}/approve")]
        [Authorize(Roles = "Admin,ClaimsOfficer")]
        public async Task<IActionResult> ApproveClaim(int claimId, [FromBody] ClaimReviewDto dto)
        {
            try
            {
                var result = await _claimService.ApproveClaimAsync(claimId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// STEP 14b: Claims Officer rejects a claim.
        /// Body: { "claimsOfficerId": 5, "notes": "Fraud suspected, insufficient evidence" }
        /// </summary>
        [HttpPost("{claimId}/reject")]
        [Authorize(Roles = "Admin,ClaimsOfficer")]
        public async Task<IActionResult> RejectClaim(int claimId, [FromBody] ClaimReviewDto dto)
        {
            // Halts progress. Fails claim workflow processing based on submitted dto properties.
            // Adjusts DB entity 'Claim' field: Status = 'Rejected'. Modifies Officer Notes property.
            try
            {
                var result = await _claimService.RejectClaimAsync(claimId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ── STEP 15: Claim Settlement (Money Granted) ─────────────────────────

        /// <summary>
        /// STEP 15: Claims Officer settles an approved claim — money is granted to customer.
        /// Claim must be in 'Approved' status first.
        /// Body: { "claimsOfficerId": 5, "settlementNotes": "Settlement processed via bank transfer" }
        /// </summary>
        // ==========================================
        // FLOW 2: THE CLAIMS LIFECYCLE
        // Step 5: Claim Settlement
        // Once approved, finance settles the claim, transferring funds to the customer and officially resolving the issue.
        // ==========================================
        [HttpPost("{claimId}/settle")]
        [Authorize(Roles = "Admin,ClaimsOfficer")]
        public async Task<IActionResult> SettleClaim(int claimId, [FromBody] ClaimSettlementDto dto)
        {
            // Ultimate terminal endpoint for granting funds to client claim request.
            // Locks the DB records processing payment pipeline execution internally via Settlement logic.
            // Modifies 'Claim' DB entity -> Status: 'Settled'
            try
            {
                var result = await _claimService.SettleClaimAsync(claimId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ── Claim Document Upload ─────────────────────────────────────────────

        /// <summary>
        /// Upload a supporting document for a claim.
        /// Customer uploads evidence (receipts, photos, reports) to support their claim.
        /// Body: { "claimId": 2, "fileName": "receipt.pdf", "filePath": "/uploads/receipt.pdf" }
        /// </summary>
        [HttpPost("{claimId}/documents")]
        [Authorize(Roles = "Customer,Admin,ClaimsOfficer")]
        public async Task<IActionResult> UploadDocument(int claimId, [FromBody] UploadClaimDocumentDto dto)
        {
            try
            {
                dto.ClaimId = claimId;
                var result = await _claimService.UploadDocumentAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Get all documents for a specific claim.</summary>
        [HttpGet("{claimId}/documents")]
        [Authorize(Roles = "Admin,ClaimsOfficer,Customer")]
        public async Task<IActionResult> GetDocuments(int claimId)
        {
            var results = await _claimService.GetDocumentsAsync(claimId);
            return Ok(results);
        }
    }
}
