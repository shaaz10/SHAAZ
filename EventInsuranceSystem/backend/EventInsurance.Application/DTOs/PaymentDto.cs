// ==========================================
// File: PaymentDto.cs
// Layer: EventInsurance.Application
// Description: Data Transfer Object (DTO) used to transfer data between the presentation layer and application layer for PaymentDto operations.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
namespace EventInsurance.Application.DTOs
{
    /// <summary>Request DTO for making a payment (Step 11).</summary>
    public class MakePaymentRequestDto
    {
        public int PolicyId { get; set; }
        public decimal Amount { get; set; }

        // Optional: customer can pass their own reference, else auto-generated
        public string? TransactionReference { get; set; }
    }

    /// <summary>Response DTO returned after a payment is processed.</summary>
    public class PaymentResponseDto
    {
        public int Id { get; set; }

        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public string Status { get; set; } = string.Empty;          // Completed / Failed / Pending
        public string TransactionReference { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
