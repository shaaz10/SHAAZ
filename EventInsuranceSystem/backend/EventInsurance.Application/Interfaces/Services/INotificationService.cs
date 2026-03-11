// ==========================================
// File: INotificationService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for INotificationService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System.Collections.Generic;
using System.Threading.Tasks;
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
        Task CreateNotificationAsync(int userId, string title, string message);
        Task DeleteNotificationAsync(int notificationId, int userId);
    }
}
