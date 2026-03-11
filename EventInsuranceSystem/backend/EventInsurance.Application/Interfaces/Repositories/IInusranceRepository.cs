// ==========================================
// File: IInusranceRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for IInusranceRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Interfaces.Repositories
{
    public interface IInsuranceRequestRepository
    {
        Task<InsuranceRequest?> GetByIdAsync(int id);
        Task<IEnumerable<InsuranceRequest>> GetAllAsync();
        Task<IEnumerable<InsuranceRequest>> GetUnassignedRequestsAsync();
        Task AddAsync(InsuranceRequest request);
        Task UpdateAsync(InsuranceRequest request);
        Task<IEnumerable<InsuranceRequest>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<InsuranceRequest>> GetByCustomerIdAsync(int customerId);
    }
}
