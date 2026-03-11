using EventInsurance.Application.Common;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Services;
using EventInsurance.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace EventInsurance.Test.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();

            _jwtOptionsMock.Setup(x => x.Value).Returns(new JwtSettings
            {
                Key = "SuperSecretKeyForTestingPurposeOnlyThatIsOver32BytesLong!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                DurationInMinutes = 60
            });

            _authService = new AuthService(_userRepoMock.Object, _jwtOptionsMock.Object);
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_AndReturnUser()
        {
            // Arrange
            var fullName = "John Doe";
            var email = "john@example.com";
            var password = "password123";
            var phone = "1234567890";

            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync(fullName, email, password, phone);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fullName, result.FullName);
            Assert.Equal(email, result.Email);
            Assert.Equal(2, result.RoleId); // Customer default
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmailWithRoleAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync("nonexistent@example.com", "pass");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIncorrect()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                PasswordHash = HashPassword("correctpassword"),
                Role = new Role { RoleName = "Customer" }
            };
            _userRepoMock.Setup(r => r.GetByEmailWithRoleAsync(user.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(user.Email, "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                PasswordHash = HashPassword("correctpassword"),
                IsActive = true,
                Role = new Role { RoleName = "Customer" }
            };
            _userRepoMock.Setup(r => r.GetByEmailWithRoleAsync(user.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(user.Email, "correctpassword");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task RegisterAsync_ShouldAssignCorrectRoleId_WhenRoleIsProvided()
        {
            // Arrange
            var fullName = "Custom Role";
            var email = "role@example.com";
            var password = "password123";
            var phone = "1234567890";
            var roleId = 3;

            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync(fullName, email, password, phone, roleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roleId, result.RoleId);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }
    }
}
