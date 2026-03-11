using EventInsurance.API.Controllers;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventInsurance.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authSvcMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authSvcMock = new Mock<IAuthService>();
            _userRepoMock = new Mock<IUserRepository>();
            _controller = new AuthController(_authSvcMock.Object, _userRepoMock.Object);
        }

        [Fact]
        public async Task Login_ValidCreds_ReturnsOkWithToken()
        {
            // Arrange
            var req = new LoginRequest("test@test.com", "Password123");
            _authSvcMock.Setup(s => s.LoginAsync(req.Email, req.Password)).ReturnsAsync("fake-jwt-token");

            // Act
            var result = await _controller.Login(req);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_InvalidCreds_ReturnsUnauthorized()
        {
            // Arrange
            var req = new LoginRequest("wrong@test.com", "fail");
            _authSvcMock.Setup(s => s.LoginAsync(req.Email, req.Password)).ReturnsAsync("");

            // Act
            var result = await _controller.Login(req);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Register_ValidData_ReturnsOk()
        {
            // Arrange
            var req = new RegisterRequest("Test User", "register@test.com", "Pass123", "1234567890");
            var user = new User { FullName = req.FullName, Email = req.Email };
            _authSvcMock.Setup(s => s.RegisterAsync(req.FullName, req.Email, req.Password, req.PhoneNumber, 2)).ReturnsAsync(user);

            // Act
            var result = await _controller.Register(req);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task CreateUser_AdminAction_ReturnsOk()
        {
            // Arrange
            var req = new CreateUserRequest("Agent User", "agent@test.com", "Pass123", "0987654321", 3);
            var user = new User { FullName = req.FullName, Email = req.Email, RoleId = req.RoleId };
            _authSvcMock.Setup(s => s.RegisterAsync(req.FullName, req.Email, req.Password, req.PhoneNumber, req.RoleId)).ReturnsAsync(user);

            // Act
            var result = await _controller.CreateUser(req);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetAgents_ReturnsListOfAgents()
        {
            // Arrange
            var users = new List<User> { 
                new User { Id = 1, FullName = "Agent A", RoleId = 3 },
                new User { Id = 2, FullName = "Customer B", RoleId = 2 }
            };
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAgents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var agents = (IEnumerable<object>)okResult.Value!;
            Assert.Single(agents);
        }
    }
}
