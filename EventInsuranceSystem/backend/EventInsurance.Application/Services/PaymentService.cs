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
    /// Supports monthly installments and pre-payments.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IActivePolicyRepository _activePolicyRepo;
        private readonly IAgentCommissionRepository _commissionRepo;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IActivePolicyRepository activePolicyRepo,
            IAgentCommissionRepository commissionRepo)
        {
            _paymentRepo = paymentRepo;
            _activePolicyRepo = activePolicyRepo;
            _commissionRepo = commissionRepo;
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

            // 3. Rounding and Pre-payment check
            // We allow payments that are multiples of InstallmentAmount
            if (policy.InstallmentAmount <= 0)
            {
                // Fallback if installments aren't setup: expect full premium once
                if (dto.Amount != policy.PremiumAmount)
                    throw new Exception($"Incorrect payment amount. Your premium is {policy.PremiumAmount:C}.");
            }
            else
            {
                // Verify amount is at least 1 installment
                if (dto.Amount < policy.InstallmentAmount - 0.01m)
                    throw new Exception($"Minimum payment amount is one installment ({policy.InstallmentAmount:C}).");
            }

            // 4. Calculate how many months this payment covers
            int monthsToAdvance = 0;
            if (policy.InstallmentAmount > 0)
            {
                monthsToAdvance = (int)Math.Round(dto.Amount / policy.InstallmentAmount);
            }

            // 5. Auto-generate transaction reference if not provided
            var txnRef = string.IsNullOrWhiteSpace(dto.TransactionReference)
                ? GenerateTransactionRef(dto.PolicyId)
                : dto.TransactionReference;

            // 6. Build payment record
            var payment = new Payment
            {
                PolicyId = dto.PolicyId,
                Amount = dto.Amount,
                PaymentDate = DateTime.UtcNow,
                Status = "Completed",
                TransactionReference = txnRef,
                CreatedAt = DateTime.UtcNow,
                InstallmentNumber = policy.InstallmentsPaid + 1 // Tracks the first month this payment covers
            };

            // 7. Update Policy State
            policy.InstallmentsPaid += monthsToAdvance;
            
            // Extend the term if paid beyond initial/fixed term
            if (policy.InstallmentsPaid > policy.TotalInstallments)
            {
                policy.TotalInstallments = policy.InstallmentsPaid;
            }

            // EndDate represents the end of the covered period
            policy.EndDate = policy.StartDate.AddMonths(policy.InstallmentsPaid);
            
            // NextPaymentDueDate is simply the end of the paid period
            // If they paid for 1 month starting March 11, the next payment is due April 11.
            policy.NextPaymentDueDate = policy.StartDate.AddMonths(policy.InstallmentsPaid);

            // Update total premium recorded
            policy.PremiumAmount = policy.InstallmentAmount * policy.TotalInstallments;

            // 8. Generate Agent Commission for this specific payment (10%)
            var commissionAmount = Math.Round(dto.Amount * 0.10m, 2);
            var commission = new AgentCommission
            {
                AgentId = policy.AgentId,
                PolicyId = policy.Id,
                CommissionAmount = commissionAmount,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow
            };

            // 9. Save
            await _paymentRepo.AddAsync(payment);
            await _commissionRepo.AddAsync(commission);
            await _activePolicyRepo.UpdateAsync(policy);

            // 9. Return response
            return new PaymentResponseDto
            {
                Id = payment.Id,
                PolicyId = dto.PolicyId,
                PolicyNumber = policy.PolicyNumber,
                Amount = dto.Amount,
                PaymentDate = payment.PaymentDate,
                Status = "Completed",
                TransactionReference = txnRef,
                Message = $"Payment of {dto.Amount:C} received successfully. Advanced coverage by {monthsToAdvance} months. Next due: {policy.NextPaymentDueDate:yyyy-MM-dd}"
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
