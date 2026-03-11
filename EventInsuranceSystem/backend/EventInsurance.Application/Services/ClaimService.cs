// ==========================================
// File: ClaimService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for ClaimService.
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
    /// STEP 12 – Customer Raises a Claim
    /// INSERT into Claims:
    ///   • PolicyId, CustomerId linked
    ///   • ClaimAmount + Description from customer
    ///   • Status = Submitted
    ///   • FraudScore = 0, FraudFlag = false (AI will update in Step 13)
    ///   • ClaimsOfficerId = 0 (assigned later by admin in Step 14)
    /// </summary>
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepo;
        private readonly IActivePolicyRepository _policyRepo;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepo;

        public ClaimService(
            IClaimRepository claimRepo,
            IActivePolicyRepository policyRepo,
            INotificationService notificationService,
            IUserRepository userRepo)
        {
            _claimRepo = claimRepo;
            _policyRepo = policyRepo;
            _notificationService = notificationService;
            _userRepo = userRepo;
        }

        /// <summary>
        /// Logic for a customer to raise a new insurance claim.
        /// Performs extensive validation (policy status, amount) and initializes fraud monitoring fields.
        /// </summary>
        public async Task<ClaimResponseDto> RaiseClaimAsync(RaiseClaimRequestDto dto)
        {
            // 1. Verify that the referenced active policy exists in the system
            var policy = await _policyRepo.GetByIdAsync(dto.PolicyId);
            if (policy == null)
                throw new Exception($"Active policy with ID {dto.PolicyId} not found.");

            // 2. Ensure the policy is currently in 'Active' status before allowing a claim
            if (policy.Status != PolicyStatus.Active)
                throw new Exception(
                    $"Claims can only be raised against Active policies. " +
                    $"Policy status is '{policy.Status}'.");

            // 3. Perform basic numerical validation on the submitted claim amount
            if (dto.ClaimAmount <= 0)
                throw new Exception("Claim amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new Exception("Claim description is required.");

            // 4. Instantiate a new Claim domain object with default submission values
            var claim = new Claim
            {
                PolicyId = dto.PolicyId,
                CustomerId = dto.CustomerId,
                ClaimsOfficerId = null,     // To be assigned by an Admin at a later stage
                ClaimAmount = dto.ClaimAmount,
                Description = dto.Description,
                FraudScore = 0,             // Initial score before AI analysis
                FraudFlag = false,          // Initial flag before AI analysis
                Status = ClaimStatus.Submitted,
                SubmittedDate = DateTime.UtcNow,
                Documents = new List<ClaimDocument>(),
                CreatedAt = DateTime.UtcNow
            };

            // 5. Commit the new claim to the database
            await _claimRepo.AddAsync(claim);

            // Notify administrative staff safely
            try
            {
                // Find the dynamic Admin user (RoleId = 1) securely
                var allUsers = await _userRepo.GetAllAsync();
                var adminUser = allUsers.FirstOrDefault(u => u.RoleId == 1);
                
                if (adminUser != null)
                {
                    await _notificationService.CreateNotificationAsync(adminUser.Id, "New Claim Submitted", $"A new claim for {dto.ClaimAmount:C} has been filed and needs officer assignment.");
                }
            }
            catch { /* Ignore notification errors to not block claim creation */ }

            // 6. Construct and return a structured response to the customer
            return new ClaimResponseDto
            {
                Id = claim.Id,
                PolicyId = dto.PolicyId,
                PolicyNumber = policy.PolicyNumber,
                CustomerId = dto.CustomerId,
                CustomerName = policy.Customer?.FullName ?? string.Empty,
                ClaimAmount = dto.ClaimAmount,
                Description = dto.Description,
                Status = "Submitted",
                FraudScore = 0,
                FraudFlag = false,
                SubmittedDate = claim.SubmittedDate,
                SettlementDate = null,
                Message = $"Claim submitted successfully. Claim ID: {claim.Id}. You will be notified once reviewed."
            };
        }

        public async Task<ClaimResponseDto?> GetByIdAsync(int id)
        {
            var c = await _claimRepo.GetByIdAsync(id);
            return c == null ? null : MapToDto(c);
        }

        public async Task<IEnumerable<ClaimResponseDto>> GetByPolicyIdAsync(int policyId)
        {
            var list = await _claimRepo.GetByPolicyIdAsync(policyId);
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<ClaimResponseDto>> GetByCustomerIdAsync(int customerId)
        {
            var list = await _claimRepo.GetByCustomerIdAsync(customerId);
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<ClaimResponseDto>> GetByStatusAsync(ClaimStatus status)
        {
            var list = await _claimRepo.GetByStatusAsync(status);
            return list.Select(MapToDto);
        }

        /// <summary>
        /// Logic for a Claims Officer to approve a submitted claim after investigation.
        /// Changes status to 'Approved' and notifies the customer.
        /// </summary>
        public async Task<ClaimResponseDto> ApproveClaimAsync(int claimId, ClaimReviewDto dto)
        {
            // Fetch the claim to be modified
            var claim = await _claimRepo.GetByIdAsync(claimId);
            if (claim == null)
                throw new Exception($"Claim {claimId} not found.");

            // Validate that the claim is in a state that allows for approval
            if (claim.Status != ClaimStatus.Submitted && claim.Status != ClaimStatus.UnderReview)
                throw new Exception($"Claim cannot be approved. Current status: {claim.Status}.");

            // Update status and track which officer performed the action
            claim.Status = ClaimStatus.Approved;
            claim.ClaimsOfficerId = dto.ClaimsOfficerId;
            claim.UpdatedAt = DateTime.UtcNow;
            
            // Persist state changes
            await _claimRepo.SaveChangesAsync();

            // Notify the customer about the positive outcome
            await _notificationService.CreateNotificationAsync(claim.CustomerId, "Claim Approved", $"Great news! Your claim #{claimId} has been approved.");

            var result = MapToDto(claim);
            result.Message = $"✅ Claim {claimId} approved by officer {dto.ClaimsOfficerId}. Ready for settlement.";
            return result;
        }

        public async Task<ClaimResponseDto> RejectClaimAsync(int claimId, ClaimReviewDto dto)
        {
            var claim = await _claimRepo.GetByIdAsync(claimId);
            if (claim == null)
                throw new Exception($"Claim {claimId} not found.");

            if (claim.Status != ClaimStatus.Submitted && claim.Status != ClaimStatus.UnderReview)
                throw new Exception($"Claim cannot be rejected. Current status: {claim.Status}.");

            claim.Status = ClaimStatus.Rejected;
            claim.ClaimsOfficerId = dto.ClaimsOfficerId;
            claim.UpdatedAt = DateTime.UtcNow;
            await _claimRepo.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(claim.CustomerId, "Claim Rejected", $"We're sorry, your claim #{claimId} was rejected. Note: {dto.Notes}");

            var result = MapToDto(claim);
            result.Message = $"❌ Claim {claimId} rejected. Reason: {dto.Notes}";
            return result;
        }

        /// <summary>
        /// Logic for final claim settlement where money is officially granted to the customer.
        /// Sets settlement date and completes the claim lifecycle.
        /// </summary>
        public async Task<ClaimResponseDto> SettleClaimAsync(int claimId, ClaimSettlementDto dto)
        {
            // Retrieve the approved claim
            var claim = await _claimRepo.GetByIdAsync(claimId);
            if (claim == null)
                throw new Exception($"Claim {claimId} not found.");

            // Verification: A claim must be Approved before it can be Settled
            if (claim.Status != ClaimStatus.Approved)
                throw new Exception(
                    $"Claim cannot be settled. Status must be 'Approved'. Current: {claim.Status}.");

            // Update terminal status and record settlement time
            claim.Status = ClaimStatus.Settled;
            claim.SettlementDate = DateTime.UtcNow;
            claim.ClaimsOfficerId = dto.ClaimsOfficerId;
            claim.UpdatedAt = DateTime.UtcNow;
            
            // Save final state
            await _claimRepo.SaveChangesAsync();

            // Provide a detailed confirmation response
            var result = MapToDto(claim);
            result.Message = $"💰 Claim {claimId} settled! Amount of {claim.ClaimAmount:C} " +
                             $"granted to customer on {claim.SettlementDate:yyyy-MM-dd}. {dto.SettlementNotes}";
            return result;
        }

        public async Task<IEnumerable<ClaimResponseDto>> GetAllAsync()
        {
            var list = await _claimRepo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<ClaimResponseDto> AssignOfficerAsync(int claimId, int claimsOfficerId)
        {
            var claim = await _claimRepo.GetByIdAsync(claimId);
            if (claim == null)
                throw new Exception($"Claim {claimId} not found.");

            if (claim.Status != ClaimStatus.Submitted)
                throw new Exception(
                    $"Officer can only be assigned to Submitted claims. Current status: {claim.Status}.");

            // Assign officer and move to UnderReview
            claim.ClaimsOfficerId = claimsOfficerId;
            claim.Status = ClaimStatus.UnderReview;
            claim.UpdatedAt = DateTime.UtcNow;
            await _claimRepo.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(claimsOfficerId, "Claim Assigned", $"You have been assigned to review claim #{claimId}.");
            await _notificationService.CreateNotificationAsync(claim.CustomerId, "Officer Assigned", $"Your claim #{claimId} is now under review by our Claims Officer.");

            var result = MapToDto(claim);
            result.Message = $"✅ Claims Officer {claimsOfficerId} assigned to claim {claimId}. Status → UnderReview.";
            return result;
        }

        public async Task<IEnumerable<ClaimResponseDto>> GetByOfficerIdAsync(int officerId)
        {
            var list = await _claimRepo.GetByOfficerIdAsync(officerId);
            return list.Select(MapToDto);
        }

        public async Task<ClaimDocumentResponseDto> UploadDocumentAsync(UploadClaimDocumentDto dto)
        {
            var claim = await _claimRepo.GetByIdAsync(dto.ClaimId);
            if (claim == null)
                throw new Exception($"Claim {dto.ClaimId} not found.");

            if (claim.Status == ClaimStatus.Settled || claim.Status == ClaimStatus.Rejected)
                throw new Exception($"Cannot upload documents on a {claim.Status} claim.");

            var doc = new ClaimDocument
            {
                ClaimId = dto.ClaimId,
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                UploadedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            claim.Documents.Add(doc);
            await _claimRepo.SaveChangesAsync();

            return new ClaimDocumentResponseDto
            {
                Id = doc.Id,
                ClaimId = dto.ClaimId,
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                UploadedAt = doc.UploadedAt,
                Message = $"Document '{dto.FileName}' uploaded to claim {dto.ClaimId}."
            };
        }

        public async Task<IEnumerable<ClaimDocumentResponseDto>> GetDocumentsAsync(int claimId)
        {
            var claim = await _claimRepo.GetByIdAsync(claimId);
            if (claim == null)
                throw new Exception($"Claim {claimId} not found.");

            return claim.Documents.Select(d => new ClaimDocumentResponseDto
            {
                Id = d.Id,
                ClaimId = claimId,
                FileName = d.FileName,
                FilePath = d.FilePath,
                UploadedAt = d.UploadedAt
            });
        }

        private static ClaimResponseDto MapToDto(Claim c)
        {
            return new ClaimResponseDto
            {
                Id = c.Id,
                PolicyId = c.PolicyId,
                PolicyNumber = c.Policy?.PolicyNumber ?? string.Empty,
                CustomerId = c.CustomerId,
                CustomerName = c.Customer?.FullName ?? string.Empty,
                ClaimAmount = c.ClaimAmount,
                Description = c.Description,
                Status = c.Status.ToString(),
                FraudScore = c.FraudScore,
                FraudFlag = c.FraudFlag,
                SubmittedDate = c.SubmittedDate,
                SettlementDate = c.SettlementDate,
                Message = string.Empty
            };
        }
    }
}
