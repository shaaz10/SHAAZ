// ==========================================
// File: AuditLog.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for AuditLog.
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
    public class AuditLog : BaseEntity
    {
        public string TableName { get; set; }
        public int RecordId { get; set; }

        public string Action { get; set; }   // Insert / Update / Delete

        public int PerformedByUserId { get; set; }
        public User PerformedByUser { get; set; }

        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }
}