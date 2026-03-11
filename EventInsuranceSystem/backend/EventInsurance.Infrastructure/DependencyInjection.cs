// ==========================================
// File: DependencyInjection.cs
// Layer: EventInsurance.Infrastructure
// Description: Component representing DependencyInjection functionality within the system.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EventInsurance.Infrastructure.Persistence;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Infrastructure.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Application.Services;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Infrastructure.Repositories;

namespace EventInsurance.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IInsuranceRequestRepository,
                   InsuranceRequestRepository>();
            services.AddScoped<IPolicySuggestionRepository,
                   PolicySuggestionRepository>();
            services.AddScoped<IPolicyApplicationRepository,
                   PolicyApplicationRepository>();
            services.AddScoped<IApplicationDocumentRepository,
                   ApplicationDocumentRepository>();
            services.AddScoped<IActivePolicyRepository,
                   ActivePolicyRepository>();
            services.AddScoped<IAgentCommissionRepository,
                   AgentCommissionRepository>();
            services.AddScoped<IPaymentRepository,
                   PaymentRepository>();
            services.AddScoped<IClaimRepository,
                   ClaimRepository>();
            services.AddScoped<IPolicyRepository,
                   PolicyRepository>();
            services.AddScoped<INotificationRepository,
                   NotificationRepository>();

            return services;
        }
    }
}