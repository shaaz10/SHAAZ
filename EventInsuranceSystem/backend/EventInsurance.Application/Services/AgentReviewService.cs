// ==========================================
// File: AgentReviewService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for AgentReviewService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Enums;

namespace EventInsurance.Application.Services
{
    public class AgentReviewService : IAgentReviewService
    {
        private readonly IPolicyApplicationRepository _applicationRepository;
        private readonly INotificationService _notificationService;

        public AgentReviewService(
            IPolicyApplicationRepository applicationRepository,
            INotificationService notificationService)
        {
            _applicationRepository = applicationRepository;
            _notificationService = notificationService;
        }

        public async Task<ApplicationReviewResponseDto> ReviewApplicationAsync(
            int applicationId,
            int agentId,
            AgentReviewApplicationDto dto)
        {
            var application = await _applicationRepository
                .GetByIdAsync(applicationId);

            if (application == null)
                throw new Exception("Application not found");

            // Verify agent is the one assigned to this application
            if (application.AgentId != agentId)
                throw new Exception("Unauthorized: You are not assigned to this application");

            // Allow review from multiple valid states
            var reviewableStatuses = new[]
            {
                PolicyApplicationStatus.PendingValidation,
                PolicyApplicationStatus.DocumentsPending,
                PolicyApplicationStatus.UnderReview
            };

            if (!reviewableStatuses.Contains(application.Status))
                throw new Exception($"Application cannot be reviewed. Current status: {application.Status}");

            // Handle escalation
            if (dto.EscalateToAdmin)
            {
                application.Status = PolicyApplicationStatus.EscalatedToAdmin;
                application.UpdatedAt = DateTime.UtcNow;
                await _applicationRepository.SaveChangesAsync();

                try
                {
                    await _notificationService.CreateNotificationAsync(application.CustomerId, "Application Escalated", $"Your application #{applicationId} is currently under final review by Admin.");
                }
                catch { /* Notification failure should not block escalation */ }

                return new ApplicationReviewResponseDto
                {
                    ApplicationId = applicationId,
                    Status = "EscalatedToAdmin",
                    ReviewNotes = dto.ReviewNotes,
                    IsEscalated = true,
                    Message = "Application escalated to admin for approval"
                };
            }

            // Handle approve / reject
            if (dto.IsApproved)
            {
                application.Status = PolicyApplicationStatus.Approved;
                application.UpdatedAt = DateTime.UtcNow;
                await _applicationRepository.SaveChangesAsync();

                try
                {
                    await _notificationService.CreateNotificationAsync(application.CustomerId, "Application Approved", $"Your application #{applicationId} has been approved.");
                }
                catch { /* Notification failure should not block approval */ }

                return new ApplicationReviewResponseDto
                {
                    ApplicationId = applicationId,
                    Status = "Approved",
                    ReviewNotes = dto.ReviewNotes,
                    IsEscalated = false,
                    Message = "Application approved by agent"
                };
            }
            else
            {
                application.Status = PolicyApplicationStatus.Rejected;
                application.UpdatedAt = DateTime.UtcNow;
                await _applicationRepository.SaveChangesAsync();

                try
                {
                    await _notificationService.CreateNotificationAsync(application.CustomerId, "Application Rejected", $"Your application #{applicationId} was rejected. Note: {dto.ReviewNotes}");
                }
                catch { /* Notification failure should not block rejection */ }

                return new ApplicationReviewResponseDto
                {
                    ApplicationId = applicationId,
                    Status = "Rejected",
                    ReviewNotes = dto.ReviewNotes,
                    IsEscalated = false,
                    Message = "Application rejected by agent"
                };
            }
        }

        public async Task<ApplicationReviewResponseDto> EscalateToAdminAsync(
            int applicationId,
            int agentId,
            string notes)
        {
            var dto = new AgentReviewApplicationDto
            {
                ReviewNotes = notes,
                EscalateToAdmin = true
            };

            return await ReviewApplicationAsync(applicationId, agentId, dto);
        }
    }
}
