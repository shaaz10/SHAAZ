// ==========================================
// File: Role.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for Role.
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
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
