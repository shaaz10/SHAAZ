// ==========================================
// File: Payment.cs
// Layer: EventInsurance.Domain
// Description: Domain Entity representing a core business concept and database table structure for Payment.
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
    public class Payment : BaseEntity
    {
        public int PolicyId { get; set; }
        public ActivePolicy Policy { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public string Status { get; set; }   // Completed / Failed / Pending
        public string TransactionReference { get; set; }
        public int? InstallmentNumber { get; set; }
    }
}
