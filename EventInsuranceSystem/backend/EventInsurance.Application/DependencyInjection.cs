// ==========================================
// File: DependencyInjection.cs
// Layer: EventInsurance.Application
// Description: Component representing DependencyInjection functionality within the system.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventInsurance.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IInsuranceRequestService, InsuranceRequestService>();
            services.AddScoped<IPolicySuggestionService, PolicySuggestionService>();
            services.AddScoped<IPolicyApplicationService, PolicyApplicationService>();
            services.AddScoped<IApplicationDocumentService, ApplicationDocumentService>();
            services.AddScoped<IAIDocumentValidationService, AIDocumentValidationService>();
            services.AddScoped<IAgentReviewService, AgentReviewService>();
            services.AddScoped<IAdminApprovalService, AdminApprovalService>();
            services.AddScoped<IActivePolicyService, ActivePolicyService>();
            services.AddScoped<IAgentCommissionService, AgentCommissionService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<IAIFraudDetectionService, AIFraudDetectionService>();
            services.AddScoped<IAITextService, AITextService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}