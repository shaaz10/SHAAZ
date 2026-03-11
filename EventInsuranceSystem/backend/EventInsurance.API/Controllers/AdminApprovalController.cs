using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminApprovalController : ControllerBase
    {
        private readonly IAdminApprovalService _service;
        private readonly IPolicyApplicationRepository _appRepo;

        public AdminApprovalController(
            IAdminApprovalService service,
            IPolicyApplicationRepository appRepo)
        {
            _service = service;
            _appRepo = appRepo;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("escalated")]
        public async Task<IActionResult> GetEscalated()
        {
            var apps = await _appRepo.GetEscalatedAsync();
            return Ok(apps);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("approved")]
        public async Task<IActionResult> GetApproved()
        {
            var apps = await _appRepo.GetApprovedAsync();
            return Ok(apps);
        }

        // ==========================================
        // FLOW 1: FROM REQUEST TO ACTIVE POLICY
        // Step 6: Agent Review & Admin Approval (Admin Phase)
        // The Administrator performs the final underwriting check to formally approve the application.
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost("approve/{applicationId}")]
        public async Task<IActionResult> ApproveApplication(
            int applicationId,
            [FromBody] AdminApprovalRequestDto dto)
        {
            var result = await _service.ApproveApplicationAsync(
                applicationId,
                dto);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("approve-and-activate/{applicationId}")]
        public async Task<IActionResult> ApproveAndActivate(
            int applicationId,
            [FromBody] AdminApprovalRequestDto dto)
        {
            try
            {
                var result = await _service.ApproveAndActivateAsync(
                    applicationId,
                    dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("reject/{applicationId}")]
        public async Task<IActionResult> RejectApplication(
            int applicationId,
            [FromBody] RejectRequestDto request)
        {
            var result = await _service.RejectApplicationAsync(
                applicationId,
                request.RejectionReason);

            return Ok(result);
        }
    }

    public record RejectRequestDto(string RejectionReason);
}

