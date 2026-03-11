// ==========================================
// File: AppliationDocument.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for AppliationDocument.
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
    public class ApplicationDocument : BaseEntity
    {
        public int ApplicationId { get; set; }
        public PolicyApplication Application { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;

        public string ValidationStatus { get; set; } = "Pending";
        public decimal ConfidenceScore { get; set; }
    }
}
