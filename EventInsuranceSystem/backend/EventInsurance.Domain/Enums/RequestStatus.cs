// ==========================================
// File: RequestStatus.cs
// Layer: EventInsurance.Domain
// Description: Enumeration defining constant values and states for RequestStatus within the domain.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EventInsurance.Domain.Enums
{
    public enum RequestStatus
    {
        Submitted,
        AgentAssigned,
        PolicySuggested,
        PolicySelected,
        Closed
    }
}
