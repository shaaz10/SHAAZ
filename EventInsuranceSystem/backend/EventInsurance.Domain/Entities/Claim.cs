// ==========================================
// File: Claim.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for Claim.
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
    public class Claim : BaseEntity
    {
        public int PolicyId { get; set; }
        public ActivePolicy Policy { get; set; }

        public int CustomerId { get; set; }
        public User Customer { get; set; }

        public int? ClaimsOfficerId { get; set; }
        public User? ClaimsOfficer { get; set; }

        public decimal ClaimAmount { get; set; }
        public string Description { get; set; }

        public decimal FraudScore { get; set; }
        public bool FraudFlag { get; set; }

        public ClaimStatus Status { get; set; }

        public DateTime SubmittedDate { get; set; }
        public DateTime? SettlementDate { get; set; }

        public ICollection<ClaimDocument> Documents { get; set; }
    }
}
