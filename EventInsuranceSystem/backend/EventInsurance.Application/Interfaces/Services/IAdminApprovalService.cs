// ==========================================
// File: IAdminApprovalService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IAdminApprovalService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.DTOs;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IAdminApprovalService
    {
        Task<AdminApprovalResponseDto> ApproveApplicationAsync(
            int applicationId,
            AdminApprovalRequestDto dto);

        Task<ActivePolicyResponseDto> ApproveAndActivateAsync(
            int applicationId,
            AdminApprovalRequestDto dto);

        Task<AdminApprovalResponseDto> RejectApplicationAsync(
            int applicationId,
            string rejectionReason);
    }
}
