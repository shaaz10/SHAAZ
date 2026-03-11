// ==========================================
// File: IPaymentService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IPaymentService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        /// <summary>STEP 11: Customer makes a payment against an active policy.</summary>
        Task<PaymentResponseDto> MakePaymentAsync(MakePaymentRequestDto dto);

        Task<PaymentResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<PaymentResponseDto>> GetByPolicyIdAsync(int policyId);
        Task<IEnumerable<PaymentResponseDto>> GetAllAsync();
    }
}
