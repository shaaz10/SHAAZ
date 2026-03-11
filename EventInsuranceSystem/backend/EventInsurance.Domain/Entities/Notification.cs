// ==========================================
// File: Notification.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for Notification.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventInsurance.Domain.Common;

namespace EventInsurance.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public User User { get; set; } = default!;

        public bool IsRead { get; set; } = false;
    }
}
