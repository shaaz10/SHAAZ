// ==========================================
// File: PolicyApplication.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for PolicyApplication.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventInsurance.Domain.Common;
using EventInsurance.Domain.Enums;

namespace EventInsurance.Domain.Entities
{
    public class PolicyApplication : BaseEntity
    {
        public int RequestId { get; set; }
        public InsuranceRequest Request { get; set; } = null!;

        public int PolicyProductId { get; set; }
        public PolicyProduct PolicyProduct { get; set; } = null!;

        public int CustomerId { get; set; }
        public User Customer { get; set; } = null!;

        public int AgentId { get; set; }
        public User Agent { get; set; } = null!;

        public decimal RiskScore { get; set; }
        public string RiskCategory { get; set; } = string.Empty;

        public decimal PremiumAmount { get; set; }
        public decimal CoverageAmount { get; set; }
        public PolicyApplicationStatus Status { get; set; }

        public ICollection<ApplicationDocument> Documents { get; set; }
            = new List<ApplicationDocument>();
    }
}
