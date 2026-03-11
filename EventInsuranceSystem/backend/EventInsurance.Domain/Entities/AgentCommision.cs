// ==========================================
// File: AgentCommision.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for AgentCommision.
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
    public class AgentCommission : BaseEntity
    {
        public int AgentId { get; set; }
        public User Agent { get; set; }

        public int PolicyId { get; set; }
        public ActivePolicy Policy { get; set; }

        public decimal CommissionAmount { get; set; }

        public bool IsPaid { get; set; } = false;
        public DateTime? PaidDate { get; set; }
    }
}
