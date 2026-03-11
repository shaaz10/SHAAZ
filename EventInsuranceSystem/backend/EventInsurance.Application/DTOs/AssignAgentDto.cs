// ==========================================
// File: AssignAgentDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for AssignAgentDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventInsurance.Application.DTOs
{
    public class AssignAgentDto
    {
        public int RequestId { get; set; }
        public int AgentId { get; set; }
    }
}