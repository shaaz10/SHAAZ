// ==========================================
// File: IClaimService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IClaimService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;
using EventInsurance.Domain.Enums;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IClaimService
    {
        /// <summary>STEP 12: Customer raises a claim against an active policy.</summary>
        Task<ClaimResponseDto> RaiseClaimAsync(RaiseClaimRequestDto dto);

        /// <summary>STEP 14: Claims Officer approves a claim after review.</summary>
        Task<ClaimResponseDto> ApproveClaimAsync(int claimId, ClaimReviewDto dto);

        /// <summary>STEP 14: Claims Officer rejects a claim after review.</summary>
        Task<ClaimResponseDto> RejectClaimAsync(int claimId, ClaimReviewDto dto);

        /// <summary>STEP 15: Claims Officer settles an approved claim — money is granted.</summary>
        Task<ClaimResponseDto> SettleClaimAsync(int claimId, ClaimSettlementDto dto);

        Task<ClaimResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ClaimResponseDto>> GetByPolicyIdAsync(int policyId);
        Task<IEnumerable<ClaimResponseDto>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<ClaimResponseDto>> GetByStatusAsync(ClaimStatus status);
        Task<IEnumerable<ClaimResponseDto>> GetAllAsync();

        /// <summary>Upload a supporting document for a claim.</summary>
        Task<ClaimDocumentResponseDto> UploadDocumentAsync(UploadClaimDocumentDto dto);

        /// <summary>Get all documents for a claim.</summary>
        Task<IEnumerable<ClaimDocumentResponseDto>> GetDocumentsAsync(int claimId);

        /// <summary>ADMIN: Assign a Claims Officer to a claim.</summary>
        Task<ClaimResponseDto> AssignOfficerAsync(int claimId, int claimsOfficerId);

        /// <summary>Get all claims assigned to a specific Claims Officer.</summary>
        Task<IEnumerable<ClaimResponseDto>> GetByOfficerIdAsync(int officerId);
    }
}
