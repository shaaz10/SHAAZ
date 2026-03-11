// ==========================================
// File: InsuranceRequest.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for InsuranceRequest.
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
    public class InsuranceRequest : BaseEntity
    {
        public int CustomerId { get; set; }
        public User Customer { get; set; }

        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public int EventDuration { get; set; }
        public string EventLocation { get; set; }
        public int EstimatedAttendees { get; set; }
        public decimal EventBudget { get; set; }
        public decimal CoverageRequested { get; set; }
        public string RiskFactors { get; set; }

        public int? AssignedAgentId { get; set; }
        public User AssignedAgent { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Submitted;

        public ICollection<PolicySuggestion> PolicySuggestions { get; set; }
    }
}
