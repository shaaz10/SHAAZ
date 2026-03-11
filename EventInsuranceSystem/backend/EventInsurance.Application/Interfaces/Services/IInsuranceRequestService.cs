// ==========================================
// File: IInsuranceRequestService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IInsuranceRequestService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventInsurance.Domain.Entities;

using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IInsuranceRequestService
    {
        Task CreateRequestAsync(
            int customerId,
            CreateInsuranceRequestDto dto);
        Task AssignAgentAsync(int requestId, int agentId);
        Task<IEnumerable<InsuranceRequest>> GetAssignedRequestsAsync(int agentId);
        Task<IEnumerable<InsuranceRequest>> GetUnassignedRequestsAsync();
        Task<IEnumerable<InsuranceRequest>> GetCustomerRequestsAsync(int customerId);
    }
}
