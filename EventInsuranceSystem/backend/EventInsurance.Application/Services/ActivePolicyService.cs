// ==========================================
// File: ActivePolicyService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for ActivePolicyService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;
using EventInsurance.Domain.Enums;

namespace EventInsurance.Application.Services
{
    /// <summary>
    /// STEP 9  – Active Policy Creation
    /// STEP 10 – Commission Generation (auto-triggered after policy creation)
    /// </summary>
    public class ActivePolicyService : IActivePolicyService
    {
        private readonly IActivePolicyRepository _activePolicyRepo;
        private readonly IPolicyApplicationRepository _applicationRepo;
        private readonly IAgentCommissionRepository _commissionRepo;
        private readonly INotificationService _notificationService;

        // Commission rate: 10% of the premium amount
        private const decimal CommissionRate = 0.10m;
        private const int DefaultInstallmentCount = 12;

        public ActivePolicyService(
            IActivePolicyRepository activePolicyRepo,
            IPolicyApplicationRepository applicationRepo,
            IAgentCommissionRepository commissionRepo,
            INotificationService notificationService)
        {
            _activePolicyRepo = activePolicyRepo;
            _applicationRepo = applicationRepo;
            _commissionRepo = commissionRepo;
            _notificationService = notificationService;
        }

        public async Task<ActivePolicyResponseDto> CreateActivePolicyAsync(int applicationId)
        {
            var application = await _applicationRepo.GetByIdWithDetailsAsync(applicationId);

            if (application == null)
                throw new Exception($"Application {applicationId} not found.");

            if (application.Status != PolicyApplicationStatus.Approved)
                throw new Exception(
                    $"Cannot create policy. Application status is '{application.Status}'. " +
                    $"Only 'Approved' applications can become active policies.");

            var existing = await _activePolicyRepo.GetByApplicationIdAsync(applicationId);
            if (existing != null)
                throw new Exception(
                    $"An active policy already exists for application {applicationId}. " +
                    $"Policy Number: {existing.PolicyNumber}");

            var policyNumber = GeneratePolicyNumber(applicationId);

            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddYears(1);

            var premiumAmount = application.PremiumAmount;
            var coverageAmount = application.CoverageAmount;

            // Calculate installment values
            var installmentAmount = Math.Round(premiumAmount / DefaultInstallmentCount, 2);

            var activePolicy = new ActivePolicy
            {
                PolicyNumber = policyNumber,
                ApplicationId = applicationId,
                CustomerId = application.CustomerId,
                AgentId = application.AgentId,
                PremiumAmount = premiumAmount,
                CoverageAmount = coverageAmount,
                StartDate = startDate,
                EndDate = endDate,
                Status = PolicyStatus.Active,
                RenewalReminderDate = endDate.AddDays(-30),
                CreatedAt = DateTime.UtcNow,
                
                // --- Installment System ---
                TotalInstallments = DefaultInstallmentCount,
                InstallmentAmount = installmentAmount,
                InstallmentsPaid = 0,
                NextPaymentDueDate = startDate.AddMonths(1) // First payment due in 1 month
            };

            await _activePolicyRepo.AddAsync(activePolicy);

            var commissionAmount = Math.Round(premiumAmount * CommissionRate, 2);
            var commission = new AgentCommission
            {
                AgentId = application.AgentId,
                PolicyId = activePolicy.Id,
                CommissionAmount = commissionAmount,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow
            };
            await _commissionRepo.AddAsync(commission);

            await _notificationService.CreateNotificationAsync(application.CustomerId, "Policy Activated!", $"Your policy {policyNumber} is now officially active.");
            await _notificationService.CreateNotificationAsync(application.AgentId, "New Commission Earned!", $"You earned a commission of {commissionAmount:C} for policy {policyNumber}.");

            return MapToDto(activePolicy);
        }

        public async Task<ActivePolicyResponseDto?> GetByIdAsync(int id)
        {
            var policy = await _activePolicyRepo.GetByIdAsync(id);
            if (policy == null) return null;
            return MapToDto(policy);
        }

        public async Task<IEnumerable<ActivePolicyResponseDto>> GetByCustomerIdAsync(int customerId)
        {
            var policies = await _activePolicyRepo.GetByCustomerIdAsync(customerId);
            return policies.Select(MapToDto);
        }

        public async Task<IEnumerable<ActivePolicyResponseDto>> GetByAgentIdAsync(int agentId)
        {
            var policies = await _activePolicyRepo.GetByAgentIdAsync(agentId);
            return policies.Select(MapToDto);
        }

        public async Task<IEnumerable<ActivePolicyResponseDto>> GetAllAsync()
        {
            var policies = await _activePolicyRepo.GetAllAsync();
            return policies.Select(MapToDto);
        }

        private static string GeneratePolicyNumber(int applicationId)
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = new Random().Next(1000, 9999);
            return $"EI-{date}-{applicationId}-{random}";
        }

        private static ActivePolicyResponseDto MapToDto(ActivePolicy p)
        {
            return new ActivePolicyResponseDto
            {
                Id = p.Id,
                PolicyNumber = p.PolicyNumber,
                ApplicationId = p.ApplicationId,
                CustomerId = p.CustomerId,
                CustomerName = p.Customer?.FullName ?? string.Empty,
                AgentId = p.AgentId,
                AgentName = p.Agent?.FullName ?? string.Empty,
                PolicyProductName = p.Application?.PolicyProduct?.Name ?? string.Empty,
                PremiumAmount = p.PremiumAmount,
                CoverageAmount = p.CoverageAmount,
                EventDate = p.Application?.Request?.EventDate,
                EventLocation = p.Application?.Request?.EventLocation ?? string.Empty,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status.ToString(),
                CreatedAt = p.CreatedAt,
                Message = string.Empty,
                
                // --- Installment System ---
                TotalInstallments = p.TotalInstallments,
                InstallmentAmount = p.InstallmentAmount,
                InstallmentsPaid = p.InstallmentsPaid,
                NextPaymentDueDate = p.NextPaymentDueDate
            };
        }
    }
}
