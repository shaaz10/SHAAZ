// ==========================================
// File: User.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for User.
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
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsLocked { get; set; } = false;
        public int FailedLoginAttempts { get; set; } = 0;

        // Navigation properties
        public ICollection<InsuranceRequest> InsuranceRequests { get; set; }
    }
}
