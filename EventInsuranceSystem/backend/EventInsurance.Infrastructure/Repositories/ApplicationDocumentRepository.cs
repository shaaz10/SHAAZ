// ==========================================
// File: ApplicationDocumentRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for ApplicationDocumentRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Domain.Entities;
using EventInsurance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventInsurance.Infrastructure.Repositories
{
    public class ApplicationDocumentRepository
        : IApplicationDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDocumentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ApplicationDocument document)
        {
            await _context.ApplicationDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationDocument?> GetByIdAsync(int id)
        {
            return await _context.ApplicationDocuments
                .Include(d => d.Application)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<ApplicationDocument>> 
            GetByApplicationIdAsync(int applicationId)
        {
            return await _context.ApplicationDocuments
                .Where(d => d.ApplicationId == applicationId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
