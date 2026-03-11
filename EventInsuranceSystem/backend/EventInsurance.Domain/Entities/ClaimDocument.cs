// ==========================================
// File: ClaimDocument.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for ClaimDocument.
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
    public class ClaimDocument : BaseEntity
    {
        public int ClaimId { get; set; }
        public Claim Claim { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}
