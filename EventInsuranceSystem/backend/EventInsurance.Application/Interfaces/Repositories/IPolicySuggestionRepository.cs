// ==========================================
// File: IPolicySuggestionRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for IPolicySuggestionRepository.
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
    public interface IPolicySuggestionRepository
    {
        Task AddAsync(PolicySuggestion suggestion);
        Task<IEnumerable<PolicySuggestion>>
    GetByRequestIdAsync(int requestId);
        Task<PolicySuggestion?> GetByIdAsync(int id);

    }
}