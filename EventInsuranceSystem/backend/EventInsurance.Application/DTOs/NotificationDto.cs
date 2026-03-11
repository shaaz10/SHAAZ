// ==========================================
// File: NotificationDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for NotificationDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;

namespace EventInsurance.Application.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
