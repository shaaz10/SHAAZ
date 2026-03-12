// ==========================================
// File: PolicySuggestionService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for PolicySuggestionService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;
using EventInsurance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Services
{
    public class PolicySuggestionService : IPolicySuggestionService
    {
        private readonly IPolicySuggestionRepository _repository;
        private readonly IInsuranceRequestRepository _requestRepository;

        public PolicySuggestionService(
            IPolicySuggestionRepository repository,
            IInsuranceRequestRepository requestRepository)
        {
            _repository = repository;
            _requestRepository = requestRepository;
        }
        public async Task<IEnumerable<PolicySuggestion>> GetSuggestionsAsync(int requestId)
        {
            return await _repository.GetByRequestIdAsync(requestId);
        }
        public async Task<IEnumerable<PolicySuggestionResponseDto>>
     GetSuggestionsByRequestIdAsync(int requestId)
        {
            var suggestions =
                await _repository.GetByRequestIdAsync(requestId);

            return suggestions.Select(s =>
                new PolicySuggestionResponseDto
                {
                    SuggestionId = s.Id,
                    SuggestedPremium = s.SuggestedPremium,
                    Status = s.Status.ToString(),

                    PolicyProductId = s.PolicyProduct.Id,
                    PolicyName = s.PolicyProduct.Name,
                    PolicyDescription = s.PolicyProduct.Description,
                    BasePremium = s.PolicyProduct.BasePremium,
                    CoverageAmount = s.PolicyProduct.CoverageAmount,
                    CustomCoverageAmount = s.CustomCoverageAmount
                });
        }
        public async Task SuggestPolicyAsync(
            int agentId,
            int requestId,
            int policyProductId,
            decimal suggestedPremium,
            decimal? customCoverageAmount = null)
        {
            // --- VALIDATION LOGIC ---
            // 1. Ensure the premium is a positive value
            if (suggestedPremium <= 0)
                throw new Exception("Suggested premium must be greater than zero.");

            // 2. Verify the request exists
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new Exception("Insurance request not found.");

            // 3. Ensure the agent is actually assigned to this request
            if (request.AssignedAgentId != agentId)
                throw new Exception("Unauthorized: You are not assigned to this insurance request.");

            var suggestion = new PolicySuggestion
            {
                RequestId = requestId,
                PolicyProductId = policyProductId,
                SuggestedByAgentId = agentId,
                SuggestedPremium = suggestedPremium,
                CustomCoverageAmount = customCoverageAmount,
                Status = SuggestionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(suggestion);
        }
    }
}