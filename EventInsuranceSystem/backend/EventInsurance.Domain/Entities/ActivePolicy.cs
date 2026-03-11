// ==========================================
// File: ActivePolicy.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for ActivePolicy.
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
    public class ActivePolicy : BaseEntity
    {
        public string PolicyNumber { get; set; }

        public int ApplicationId { get; set; }
        public PolicyApplication Application { get; set; }

        public int CustomerId { get; set; }
        public User Customer { get; set; }

        public int AgentId { get; set; }
        public User Agent { get; set; }

        public decimal PremiumAmount { get; set; }
        public decimal CoverageAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public PolicyStatus Status { get; set; }

        public DateTime? RenewalReminderDate { get; set; }
    }
}
