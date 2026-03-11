// ==========================================
// File: JwtSettings.cs
// Layer: EventInsurance.Domain
// Description: Component representing JwtSettings functionality within the system.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.Common
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
    }
}