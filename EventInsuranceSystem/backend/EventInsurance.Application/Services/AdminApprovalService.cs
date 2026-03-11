// ==========================================
// File: AdminApprovalService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for AdminApprovalService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Enums;

namespace EventInsurance.Application.Services
{
    public class AdminApprovalService : IAdminApprovalService
    {
        private readonly IPolicyApplicationRepository _applicationRepository;
        private readonly INotificationService _notificationService;

        public AdminApprovalService(
            IPolicyApplicationRepository applicationRepository,
            INotificationService notificationService)
        {
            _applicationRepository = applicationRepository;
            _notificationService = notificationService;
        }

        public async Task<AdminApprovalResponseDto> ApproveApplicationAsync(
            int applicationId,
            AdminApprovalRequestDto dto)
        {
            // Get application
            var application = await _applicationRepository
                .GetByIdAsync(applicationId);

            if (application == null)
                throw new Exception("Application not found");

            // Verify status is EscalatedToAdmin
            if (application.Status != PolicyApplicationStatus.EscalatedToAdmin)
                throw new Exception($"Application cannot be approved. Current status: {application.Status}. Only escalated applications can be approved.");

            // Update application
            application.Status = PolicyApplicationStatus.Approved;
            application.UpdatedAt = DateTime.UtcNow;

            await _applicationRepository.SaveChangesAsync();

            try
            {
                await _notificationService.CreateNotificationAsync(application.AgentId, "Application Approved by Admin", $"Good job! Application #{applicationId} you managed has been approved by the Admin.");
                await _notificationService.CreateNotificationAsync(application.CustomerId, "Application Approved", $"Great news! Your application #{applicationId} has received final approval from the Admin.");
            }
            catch { /* Notification failure should not block */ }

            return new AdminApprovalResponseDto
            {
                ApplicationId = applicationId,
                Status = "Approved",
                IsApproved = true,
                ApprovalNotes = dto.ApprovalNotes,
                Message = "Application approved successfully. Ready for policy creation.",
                ApprovedAt = DateTime.UtcNow
            };
        }

        public async Task<AdminApprovalResponseDto> RejectApplicationAsync(
            int applicationId,
            string rejectionReason)
        {
            // Get application
            var application = await _applicationRepository
                .GetByIdAsync(applicationId);

            if (application == null)
                throw new Exception("Application not found");

            // Verify status is EscalatedToAdmin
            if (application.Status != PolicyApplicationStatus.EscalatedToAdmin)
                throw new Exception($"Application cannot be rejected. Current status: {application.Status}. Only escalated applications can be rejected.");

            // Update application
            application.Status = PolicyApplicationStatus.Rejected;
            application.RiskCategory = rejectionReason;  // Store reason in RiskCategory for now
            application.UpdatedAt = DateTime.UtcNow;

            await _applicationRepository.SaveChangesAsync();

            try
            {
                await _notificationService.CreateNotificationAsync(application.AgentId, "Application Rejected by Admin", $"Application #{applicationId} you escalated was rejected by the Admin.");
                await _notificationService.CreateNotificationAsync(application.CustomerId, "Application Rejected", $"We're sorry! Your application #{applicationId} has been rejected by the Administration. Reason: {rejectionReason}");
            }
            catch { /* Notification failure should not block */ }

            return new AdminApprovalResponseDto
            {
                ApplicationId = applicationId,
                Status = "Rejected",
                IsApproved = false,
                ApprovalNotes = rejectionReason,
                Message = "Application rejected. Customer has been notified.",
                ApprovedAt = DateTime.UtcNow
            };
        }
    }
}
