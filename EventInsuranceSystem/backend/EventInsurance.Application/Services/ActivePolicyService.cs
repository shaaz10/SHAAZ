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

        /// <summary>
        /// Orchestrates the transformation of an approved application into an active insurance policy.
        /// This involves creating the policy record, generating a unique policy number, and auto-calculating agent commission.
        /// </summary>
        public async Task<ActivePolicyResponseDto> CreateActivePolicyAsync(int applicationId)
        {
            // 1. Retrieve the detailed application record ensuring all necessary associations (Customer, Agent, Product) are loaded
            var application = await _applicationRepo.GetByIdWithDetailsAsync(applicationId);

            if (application == null)
                throw new Exception($"Application {applicationId} not found.");

            // 2. State Guard: Only applications that have passed all approval checks (AI & Manual) can be activated
            if (application.Status != PolicyApplicationStatus.Approved)
                throw new Exception(
                    $"Cannot create policy. Application status is '{application.Status}'. " +
                    $"Only 'Approved' applications can become active policies.");

            // 3. Idempotency Check: Prevent duplicate active policies for the same application ID
            var existing = await _activePolicyRepo.GetByApplicationIdAsync(applicationId);
            if (existing != null)
                throw new Exception(
                    $"An active policy already exists for application {applicationId}. " +
                    $"Policy Number: {existing.PolicyNumber}");

            // 4. Algorithmically generate a traceable unique Policy Number
            var policyNumber = GeneratePolicyNumber(applicationId);

            // 5. Establish the coverage period (Current standard: 365 days from activation)
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddYears(1);

            // 6. Finalize financial values from the approved contract stage
            var premiumAmount = application.PremiumAmount;
            var coverageAmount = application.CoverageAmount;

            // 7. Instantiate the core ActivePolicy domain model
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
                RenewalReminderDate = endDate.AddDays(-30), // Trigger reminder logic one month prior to expiration
                CreatedAt = DateTime.UtcNow
            };

            // 8. Commit the ActivePolicy to the database repository
            await _activePolicyRepo.AddAsync(activePolicy);

            // ── STEP 10: Automatic Agent Commission Protocol ─────────────────
            // Calculates the earnings for the agent based on the established Premium and CommissionRate constant
            var commissionAmount = Math.Round(premiumAmount * CommissionRate, 2);
            var commission = new AgentCommission
            {
                AgentId = application.AgentId,
                PolicyId = activePolicy.Id,
                CommissionAmount = commissionAmount,
                IsPaid = false, // Record remains unpaid until processed by finance/claims officer
                CreatedAt = DateTime.UtcNow
            };
            await _commissionRepo.AddAsync(commission);
            // ─────────────────────────────────────────────────────────────────

            // Dispatch real-time notifications to stakeholders about the successful activation
            await _notificationService.CreateNotificationAsync(application.CustomerId, "Policy Activated!", $"Your policy {policyNumber} is now officially active.");
            await _notificationService.CreateNotificationAsync(application.AgentId, "New Commission Earned!", $"You earned a commission of {commissionAmount:C} for policy {policyNumber}.");

            // 9. Format and return the final response DTO with all calculated metadata
            return new ActivePolicyResponseDto
            {
                Id = activePolicy.Id,
                PolicyNumber = activePolicy.PolicyNumber,
                ApplicationId = applicationId,
                CustomerId = application.CustomerId,
                CustomerName = application.Customer?.FullName ?? string.Empty,
                AgentId = application.AgentId,
                AgentName = application.Agent?.FullName ?? string.Empty,
                PolicyProductName = application.PolicyProduct?.Name ?? string.Empty,
                PremiumAmount = premiumAmount,
                CoverageAmount = coverageAmount,
                EventDate = application.Request?.EventDate,
                EventLocation = application.Request?.EventLocation ?? string.Empty,
                StartDate = startDate,
                EndDate = endDate,
                Status = "Active",
                CreatedAt = activePolicy.CreatedAt,
                Message = $"Active policy created. Commission of {commissionAmount:C} generated for agent."
            };
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

        // ── Private helpers ───────────────────────────────────────────────────

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
                Message = string.Empty
            };
        }
    }
}
