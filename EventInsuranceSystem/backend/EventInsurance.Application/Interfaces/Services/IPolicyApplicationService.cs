// ==========================================
// File: IPolicyApplicationService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IPolicyApplicationService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IPolicyApplicationService
    {
        Task<int> CreateApplicationAsync(
            int suggestionId,
            int customerId);
        Task<IEnumerable<EventInsurance.Domain.Entities.PolicyApplication>> GetApplicationsByAgentIdAsync(int agentId);
        Task<IEnumerable<EventInsurance.Domain.Entities.PolicyApplication>> GetApplicationsByCustomerIdAsync(int customerId);
    }
}
