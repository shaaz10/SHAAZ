// ==========================================
// File: PaymentService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for PaymentService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Services
{
    /// <summary>
    /// STEP 11 – Customer Makes Payment
    /// INSERT into Payments table:
    ///   • PolicyId linked to the active policy
    ///   • Amount = premium (or custom amount)
    ///   • PaymentDate = now
    ///   • Status = 'Completed'
    ///   • TransactionReference auto-generated (TXN-YYYYMMDD-HHmmss-<PolicyId>)
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IActivePolicyRepository _activePolicyRepo;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IActivePolicyRepository activePolicyRepo)
        {
            _paymentRepo = paymentRepo;
            _activePolicyRepo = activePolicyRepo;
        }

        public async Task<PaymentResponseDto> MakePaymentAsync(MakePaymentRequestDto dto)
        {
            // 1. Verify the active policy exists
            var policy = await _activePolicyRepo.GetByIdAsync(dto.PolicyId);
            if (policy == null)
                throw new Exception($"Active policy with ID {dto.PolicyId} not found.");

            // 2. Policy must be Active to accept payments
            if (policy.Status != Domain.Enums.PolicyStatus.Active)
                throw new Exception(
                    $"Policy {policy.PolicyNumber} is not Active (Status: {policy.Status}). Payments are only accepted for active policies.");

            // 3. Amount must exactly match the policy premium
            if (dto.Amount != policy.PremiumAmount)
                throw new Exception($"Incorrect payment amount. Your premium is {policy.PremiumAmount:C}, but you tried to pay {dto.Amount:C}.");

            // 4. Auto-generate transaction reference if not provided
            var txnRef = string.IsNullOrWhiteSpace(dto.TransactionReference)
                ? GenerateTransactionRef(dto.PolicyId)
                : dto.TransactionReference;

            // 5. Build payment record
            var payment = new Payment
            {
                PolicyId = dto.PolicyId,
                Amount = dto.Amount,
                PaymentDate = DateTime.UtcNow,
                Status = "Completed",
                TransactionReference = txnRef,
                CreatedAt = DateTime.UtcNow
            };

            // 6. Save
            await _paymentRepo.AddAsync(payment);

            // 7. Return response
            return new PaymentResponseDto
            {
                Id = payment.Id,
                PolicyId = dto.PolicyId,
                PolicyNumber = policy.PolicyNumber,
                Amount = dto.Amount,
                PaymentDate = payment.PaymentDate,
                Status = "Completed",
                TransactionReference = txnRef,
                Message = $"Payment of {dto.Amount:C} received successfully. Ref: {txnRef}"
            };
        }

        public async Task<PaymentResponseDto?> GetByIdAsync(int id)
        {
            var p = await _paymentRepo.GetByIdAsync(id);
            return p == null ? null : MapToDto(p);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetByPolicyIdAsync(int policyId)
        {
            var list = await _paymentRepo.GetByPolicyIdAsync(policyId);
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllAsync()
        {
            var list = await _paymentRepo.GetAllAsync();
            return list.Select(MapToDto);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string GenerateTransactionRef(int policyId)
        {
            var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
            return $"TXN-{stamp}-P{policyId}";
        }

        private static PaymentResponseDto MapToDto(Payment p)
        {
            return new PaymentResponseDto
            {
                Id = p.Id,
                PolicyId = p.PolicyId,
                PolicyNumber = p.Policy?.PolicyNumber ?? string.Empty,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Status = p.Status,
                TransactionReference = p.TransactionReference,
                Message = string.Empty
            };
        }
    }
}
