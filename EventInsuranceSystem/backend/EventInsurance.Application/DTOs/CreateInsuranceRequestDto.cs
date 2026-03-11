// ==========================================
// File: CreateInsuranceRequestDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for CreateInsuranceRequestDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventInsurance.Application.DTOs
{
    public class CreateInsuranceRequestDto
    {
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public int EventDuration { get; set; }
        public string EventLocation { get; set; }
        public int EstimatedAttendees { get; set; }
        public decimal EventBudget { get; set; }
        public decimal CoverageRequested { get; set; }
        public string RiskFactors { get; set; }
    }
}
