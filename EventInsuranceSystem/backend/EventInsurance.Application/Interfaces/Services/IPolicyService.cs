// ==========================================
// File: IPolicyService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IPolicyService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IPolicyService
    {
        Task SuggestPolicyAsync(PolicySuggestion suggestion);
        Task SelectPolicyAsync(int suggestionId);
        Task EscalateToAdminAsync(int applicationId);
        Task ApproveApplicationAsync(int applicationId);
    }
}
