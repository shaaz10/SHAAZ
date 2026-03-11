// ==========================================
// File: IAuthService.cs
// Layer: EventInsurance.Application
// Description: Service Interface defining business logic contracts for IAuthService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventInsurance.Domain.Entities;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(string fullName, string email, string password, string phoneNumber, int roleId = 2);
        Task<string?> LoginAsync(string email, string password);
    }
}
