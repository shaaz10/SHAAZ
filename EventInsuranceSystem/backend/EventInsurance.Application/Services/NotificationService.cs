// ==========================================
// File: NotificationService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for NotificationService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;

        public NotificationService(INotificationRepository notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepo.GetByUserIdAsync(userId);
            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);
            if (notification != null && notification.UserId == userId)
            {
                notification.IsRead = true;
                await _notificationRepo.UpdateAsync(notification);
                await _notificationRepo.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await _notificationRepo.GetByUserIdAsync(userId);
            foreach (var notification in notifications.Where(n => !n.IsRead))
            {
                notification.IsRead = true;
                await _notificationRepo.UpdateAsync(notification);
            }
            await _notificationRepo.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(int userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationRepo.AddAsync(notification);
            await _notificationRepo.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);
            if (notification != null && notification.UserId == userId)
            {
                await _notificationRepo.DeleteAsync(notification);
                await _notificationRepo.SaveChangesAsync();
            }
        }
    }
}
