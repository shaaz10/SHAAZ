// ==========================================
// File: PolicyProduct.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for PolicyProduct.
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
    public class PolicyProduct : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePremium { get; set; }
        public decimal CoverageAmount { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public bool IsApprovedByAdmin { get; set; }

        public int? ApprovedByAdminId { get; set; }
        public User ApprovedByAdmin { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<PolicySuggestion> PolicySuggestions { get; set; }
    }
}
