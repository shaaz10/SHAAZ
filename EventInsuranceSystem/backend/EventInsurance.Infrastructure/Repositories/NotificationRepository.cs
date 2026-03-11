// ==========================================
// File: NotificationRepository.cs
// Layer: EventInsurance.Infrastructure
// Description: Infrastructure Repository implementing data access operations using Entity Framework Core for NotificationRepository.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventInsurance.Domain.Entities;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Infrastructure.Persistence;

namespace EventInsurance.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
        }

        public async Task DeleteAsync(Notification notification)
        {
            _context.Notifications.Remove(notification);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
