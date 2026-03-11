// ==========================================
// File: PolicyApplicatonStatus.cs
// Layer: EventInsurance.Domain
// Description: Enumeration defining constant values and states for PolicyApplicatonStatus within the domain.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventInsurance.Domain.Enums
{
    public enum PolicyApplicationStatus
    {
        PendingValidation = 1,
        DocumentsPending = 2,
        UnderReview = 3,
        Approved = 4,
        Rejected = 5,
        EscalatedToAdmin = 6
    }
}
