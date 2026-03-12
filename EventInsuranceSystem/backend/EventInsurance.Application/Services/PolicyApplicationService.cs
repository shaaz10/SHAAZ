// ==========================================
// File: PolicyApplicationService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for PolicyApplicationService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;
using EventInsurance.Domain.Enums;


namespace EventInsurance.Application.Services
{
    public class PolicyApplicationService
        : IPolicyApplicationService
    {
        private readonly IPolicySuggestionRepository _suggestionRepository;
        private readonly IPolicyApplicationRepository _applicationRepository;
        private readonly IInsuranceRequestRepository _requestRepository;
        private readonly INotificationService _notificationService;

        public PolicyApplicationService(
            IPolicySuggestionRepository suggestionRepository,
            IPolicyApplicationRepository applicationRepository,
            IInsuranceRequestRepository requestRepository,
            INotificationService notificationService)
        {
            _suggestionRepository = suggestionRepository;
            _applicationRepository = applicationRepository;
            _requestRepository = requestRepository;
            _notificationService = notificationService;
        }

        public async Task<int> CreateApplicationAsync(
            int suggestionId,
            int customerId)
        {
            var suggestion =
                await _suggestionRepository
                    .GetByIdAsync(suggestionId);

            if (suggestion == null)
                throw new Exception("Suggestion not found");

            if (suggestion.Request.CustomerId != customerId)
                throw new Exception("Unauthorized selection");

            var application = new PolicyApplication
            {
                RequestId = suggestion.RequestId,
                PolicyProductId = suggestion.PolicyProductId,
                CustomerId = customerId,
                AgentId = suggestion.SuggestedByAgentId,
                PremiumAmount = suggestion.SuggestedPremium,
                CoverageAmount = suggestion.CustomCoverageAmount ?? suggestion.PolicyProduct?.CoverageAmount ?? 0,
                RiskScore = 0,
                RiskCategory = "Pending",
                Status = PolicyApplicationStatus.PendingValidation,
                CreatedAt = DateTime.UtcNow
            };

            await _applicationRepository.AddAsync(application);

            // Update the Request status to PolicySelected so it drops off active queues
            var request = suggestion.Request;
            request.Status = RequestStatus.PolicySelected;
            await _requestRepository.UpdateAsync(request);

            // Notify Agent
            await _notificationService.CreateNotificationAsync(suggestion.SuggestedByAgentId, "New Application Created", $"A customer has accepted your policy suggestion for request #{suggestion.RequestId}. Please review their application.");

            return application.Id;
        }

        public async Task<IEnumerable<PolicyApplication>> GetApplicationsByAgentIdAsync(int agentId)
        {
            return await _applicationRepository.GetByAgentIdAsync(agentId);
        }

        public async Task<IEnumerable<PolicyApplication>> GetApplicationsByCustomerIdAsync(int customerId)
        {
            return await _applicationRepository.GetByCustomerIdAsync(customerId);
        }
    }
}