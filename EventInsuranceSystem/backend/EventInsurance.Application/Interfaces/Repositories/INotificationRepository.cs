// ==========================================
// File: INotificationRepository.cs
// Layer: EventInsurance.Application
// Description: Repository Interface defining data access contracts for INotificationRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System.Collections.Generic;
using System.Threading.Tasks;
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task DeleteAsync(Notification notification);
        Task SaveChangesAsync();
    }
}
