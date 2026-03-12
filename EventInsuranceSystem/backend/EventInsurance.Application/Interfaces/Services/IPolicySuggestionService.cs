// ==========================================
// File: IPolicySuggestionService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IPolicySuggestionService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventInsurance.Application.DTOs;


namespace EventInsurance.Application.Interfaces.Services
{
    public interface IPolicySuggestionService
    {
        Task SuggestPolicyAsync(
            int agentId,
            int requestId,
            int policyProductId,
            decimal suggestedPremium,
            decimal? customCoverageAmount = null);
        Task<IEnumerable<PolicySuggestionResponseDto>>
     GetSuggestionsByRequestIdAsync(int requestId);
        
    }

}