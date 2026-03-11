// ==========================================
// File: IPolicyApplicationRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for IPolicyApplicationRepository.
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
    public interface IPolicyApplicationRepository
    {
        Task AddAsync(PolicyApplication application);

        Task<PolicyApplication?> GetByIdAsync(int id);

        /// <summary>Loads application with Customer, Agent, and PolicyProduct navigation properties.</summary>
        Task<PolicyApplication?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<PolicyApplication>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<PolicyApplication>> GetByAgentIdAsync(int agentId);

        Task SaveChangesAsync();
        Task<IEnumerable<PolicyApplication>> GetEscalatedAsync();
        Task<IEnumerable<PolicyApplication>> GetApprovedAsync();
    }
}
