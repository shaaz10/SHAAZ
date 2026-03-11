// ==========================================
// File: InsuranceRequestService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for InsuranceRequestService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;
using EventInsurance.Domain.Enums;

namespace EventInsurance.Application.Services
{
    public class InsuranceRequestService : IInsuranceRequestService
    {
        private readonly IInsuranceRequestRepository _repository;
        private readonly INotificationService _notificationService;

        public InsuranceRequestService(
            IInsuranceRequestRepository repository,
            INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Logic for a customer creating a new insurance request.
        /// Maps the DTO to the Domain Entity, sets initial status, and saves to DB.
        /// </summary>
        public async Task CreateRequestAsync(
            int customerId,
            CreateInsuranceRequestDto dto)
        {
            // --- VALIDATION ---
            // Ensure the event date is not in the past (Allowing today's date)
            if (dto.EventDate.Date < DateTime.UtcNow.Date)
            {
                throw new Exception("Event date cannot be in the past.");
            }

            // Map the incoming DTO properties to a new domain model instance
            var request = new InsuranceRequest
            {
                CustomerId = customerId,
                EventType = dto.EventType,
                EventDate = dto.EventDate,
                EventDuration = dto.EventDuration,
                EventLocation = dto.EventLocation,
                EstimatedAttendees = dto.EstimatedAttendees,
                EventBudget = dto.EventBudget,
                CoverageRequested = dto.CoverageRequested,
                RiskFactors = dto.RiskFactors,
                Status = 0,
                CreatedAt = DateTime.UtcNow
            };

            // Persist the new request object using the repository
            try
            {
                await _repository.AddAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot save request: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }

            // Send notification to the customer confirming submission
            // Wrapped in try-catch so notification failures don't block request creation
            try
            {
                await _notificationService.CreateNotificationAsync(
                    customerId,
                    "Request Submitted",
                    $"Your insurance request for {dto.EventType} has been submitted successfully and is pending agent assignment.");
            }
            catch { /* Notification failure should not block the request */ }
        }

        /// <summary>
        /// Logic for assigning a specific agent to an insurance request.
        /// Updates request status, link to agent, and notifies both the agent and customer.
        /// </summary>
        public async Task AssignAgentAsync(int requestId, int agentId)
        {
            // Retrieve the existing request from the database
            var request = await _repository.GetByIdAsync(requestId);

            // Error handling if the requested record doesn't exist
            if (request == null)
                throw new Exception("Request not found");

            // Update assignment and status properties
            request.AssignedAgentId = agentId;
            request.Status = RequestStatus.AgentAssigned;

            // Save the modified entity back to the database
            await _repository.UpdateAsync(request);

            // Trigger notifications (wrapped in try-catch for safety)
            try
            {
                await _notificationService.CreateNotificationAsync(
                    agentId, "New Request Assigned",
                    $"You have been assigned to review request #{requestId}.");

                await _notificationService.CreateNotificationAsync(
                    request.CustomerId, "Agent Assigned",
                    $"An agent has been assigned to your request and will review it shortly.");
            }
            catch { /* Notification failure should not block the assignment */ }
        }

        public async Task<IEnumerable<InsuranceRequest>> GetAssignedRequestsAsync(int agentId)
        {
            return await _repository.GetByAgentIdAsync(agentId);
        }

        public async Task<IEnumerable<InsuranceRequest>> GetUnassignedRequestsAsync()
        {
            return await _repository.GetUnassignedRequestsAsync();
        }

        public async Task<IEnumerable<InsuranceRequest>> GetCustomerRequestsAsync(int customerId)
        {
            return await _repository.GetByCustomerIdAsync(customerId);
        }
    }
}