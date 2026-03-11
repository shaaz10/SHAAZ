// ==========================================
// File: IPolicyRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for IPolicyRepository.
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
    public interface IPolicyRepository
    {
        Task<PolicyProduct> GetProductByIdAsync(int id);
        Task<IEnumerable<PolicyProduct>> GetAllProductsAsync();
        Task AddProductAsync(PolicyProduct product);

        Task<PolicySuggestion> GetSuggestionByIdAsync(int id);
        Task AddSuggestionAsync(PolicySuggestion suggestion);

        Task<PolicyApplication> GetApplicationByIdAsync(int id);
        Task AddApplicationAsync(PolicyApplication application);
        Task UpdateApplicationAsync(PolicyApplication application);

        Task AddActivePolicyAsync(ActivePolicy policy);
    }
}