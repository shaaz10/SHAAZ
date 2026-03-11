using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Retrieves all notifications for the currently logged-in user.
        /// Useful for real-time alerts about policy status or claim updates.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            // Extract the User ID from the JWT token claims
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            // Fetch notifications from the service for the authenticated user
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Marks a specific notification as 'read' for the authenticated user.
        /// This helps declutter the user's notification list.
        /// </summary>
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            // Verify current user authentication and ID
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            // Instruct the service to update the specified notification's read status
            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok(new { message = "Notification marked as read" });
        }

        /// <summary>
        /// Sets the status of all current notifications for the user to 'read'.
        /// Provides a quick way to clear all alerts at once.
        /// </summary>
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            // Ensure the user is authenticated and extract their ID
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            // Batch update all notifications for the user in the database
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { message = "All notifications marked as read" });
        }

        /// <summary>
        /// Deletes a specific notification for the authenticated user.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            await _notificationService.DeleteNotificationAsync(id, userId);
            return Ok(new { message = "Notification deleted" });
        }
    }
}
