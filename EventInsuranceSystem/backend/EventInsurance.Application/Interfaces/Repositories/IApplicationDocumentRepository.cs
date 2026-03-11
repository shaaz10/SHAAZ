// ==========================================
// File: IApplicationDocumentRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for IApplicationDocumentRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Interfaces.Repositories
{
    public interface IApplicationDocumentRepository
    {
        Task AddAsync(ApplicationDocument document);

        Task<ApplicationDocument?> GetByIdAsync(int id);

        Task<IEnumerable<ApplicationDocument>> 
            GetByApplicationIdAsync(int applicationId);

        Task SaveChangesAsync();
    }
}
