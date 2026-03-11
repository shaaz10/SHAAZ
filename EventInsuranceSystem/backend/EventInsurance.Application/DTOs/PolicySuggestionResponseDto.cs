// ==========================================
// File: PolicySuggestionResponseDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for PolicySuggestionResponseDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventInsurance.Application.DTOs
{
    public class PolicySuggestionResponseDto
    {
        public int SuggestionId { get; set; }
        public decimal SuggestedPremium { get; set; }
        public string Status { get; set; }

        public int PolicyProductId { get; set; }
        public string PolicyName { get; set; }
        public string PolicyDescription { get; set; }
        public decimal BasePremium { get; set; }
        public decimal CoverageAmount { get; set; }
    }
}
