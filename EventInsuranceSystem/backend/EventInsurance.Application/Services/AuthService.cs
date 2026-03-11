// ==========================================
// File: AuthService.cs
// Layer: EventInsurance.Application
// Description: Application Service containing the core business logic and orchestration for AuthService.
// This file contributes to the overall system flow by isolating its specific responsibility as part of the N-Tier Architecture.
// ==========================================
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Application.Common;
using EventInsurance.Domain.Entities;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EventInsurance.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IUserRepository userRepository,
            IOptions<JwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// Registers a new user into the system. 
        /// Hashes the password for secure storage and defaults to the 'Customer' role unless an Admin specifies otherwise.
        /// </summary>
        public async Task<User> RegisterAsync(
      string fullName,
      string email,
      string password,
      string phoneNumber,
      int roleId = 2)
        {
            // Convert the plain-text password into a one-way SHA256 hash
            var hashedPassword = HashPassword(password);

            // Prepare the domain model for the new user record
            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = hashedPassword,
                PhoneNumber = phoneNumber,
                RoleId = roleId,
                IsActive = true
            };

            // Save the user record to the persistence layer via the repository
            await _userRepository.AddAsync(user);
            return user;
        }

        /// <summary>
        /// Authenticates a user based on email and password.
        /// If valid, generates a signed JWT token containing user identity and role claims.
        /// </summary>
        public async Task<string?> LoginAsync(
            string email,
            string password)
        {
            // 1. Fetch user by email to retrieve their stored password hash and associated role
            var user = await _userRepository.GetByEmailWithRoleAsync(email);

            // Handle invalid user scenario
            if (user == null)
            {
                return null;
            }

            // 2. Validate the provided password against the stored secure hash
            var isPasswordValid = VerifyPassword(password, user.PasswordHash);
            
            if (!isPasswordValid)
            {
                return null;
            }

            // 3. Define the security claims that will be encoded into the JWT
            var claims = new List<System.Security.Claims.Claim>
{
    new System.Security.Claims.Claim(
        ClaimTypes.NameIdentifier,
        user.Id.ToString()),

    new System.Security.Claims.Claim(
        ClaimTypes.Name,
        user.FullName ?? user.Email),

    new System.Security.Claims.Claim(
        ClaimTypes.Email,
        user.Email),

    new System.Security.Claims.Claim(
        ClaimTypes.Role,
        user.Role.RoleName)
};
            // 4. Set up security key and credentials for signing the token
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            // 5. Construct the JWT security token object with expiry and audience settings
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _jwtSettings.DurationInMinutes),
                signingCredentials: creds);

            // 6. Serialize the token into a compact string format ready for the response
            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }
}