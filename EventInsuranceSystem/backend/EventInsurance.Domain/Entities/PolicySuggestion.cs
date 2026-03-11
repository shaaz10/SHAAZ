// ==========================================
// File: PolicySuggestion.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for PolicySuggestion.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Domain.Common;
using EventInsurance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventInsurance.Domain.Enums;

namespace EventInsurance.Domain.Entities
{
    public class PolicySuggestion : BaseEntity
    {
        public int RequestId { get; set; }
        public InsuranceRequest Request { get; set; }

        public int PolicyProductId { get; set; }
        public PolicyProduct PolicyProduct { get; set; }

        public int SuggestedByAgentId { get; set; }
        public User SuggestedByAgent { get; set; }

        public decimal SuggestedPremium { get; set; }

        public SuggestionStatus Status { get; set; }  // Suggested / SelectedByCustomer / Rejected
    }
}
