using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly EventInsurance.Application.Interfaces.Repositories.IUserRepository _userRepository;

        public AuthController(IAuthService authService, EventInsurance.Application.Interfaces.Repositories.IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                // Validation: Check if the user email already exists in the Database
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    // Demonstrating a Backend Business Rule violation converting into a clean 400 response
                    return BadRequest(new { message = "User with this email already exists." });
                }

                // Validates non-authenticated request. Hands routing data to AuthService instance directly.
                // Underlines Entity Creation explicitly storing internal unprivileged 'Customer' Role identity mapping Hash logic.
                var user = await _authService.RegisterAsync(
                    request.FullName,
                    request.Email,
                    request.Password,
                    request.PhoneNumber);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==========================================
        // FLOW 0: AUTHENTICATION FLOW
        // Step 2-5: The AuthService handles checking the User repository, hashing passwords, and generating JWT tokens.
        // Returns the JWT on successful credential matching.
        // ==========================================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // Parses unauthenticated credential payloads to check Database matching hash outputs.
            // On Success: Issues uniquely timed JsonWebBearer Token (JWT) mapped securely with distinct Claim Roles (Admin, Agent, Customer).
            var result = await _authService.LoginAsync(
                request.Email,
                request.Password);
            
            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(new { token = result });
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            // JWT Authorized specifically exclusively for Admins targeting Role assignments (creating Agents / Claim Officers).
            // Updates 'User' DB Entity with specified precise Roles IDs passed in CreateUserRequest.
            var user = await _authService.RegisterAsync(
    request.FullName,
    request.Email,
    request.Password,
    request.PhoneNumber,
    request.RoleId);

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("agents")]
        public async Task<IActionResult> GetAgents()
        {
            // Queries ALL users, filters mapped exclusively to RoleID=3 ('Agent').
            // Returns formatted array objects utilized closely within Frontend Agent assignment dropdowns.
            var users = await _userRepository.GetAllAsync();
            var agents = users.Where(u => u.RoleId == 3).Select(u => new { id = u.Id, name = u.FullName }).ToList();
            return Ok(agents);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("claimsofficers")]
        public async Task<IActionResult> GetClaimsOfficers()
        {
            var users = await _userRepository.GetAllAsync();
            var officers = users.Where(u => u.RoleId == 4).Select(u => new { id = u.Id, name = u.FullName }).ToList();
            return Ok(officers);
        }
    }

    public record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    string PhoneNumber);
    public record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    string PhoneNumber,
    int RoleId);

    public record LoginRequest(
        string Email,
        string Password);
}
