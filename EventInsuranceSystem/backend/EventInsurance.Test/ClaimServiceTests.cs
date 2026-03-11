using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Application.Services;
using EventInsurance.Domain.Entities;
using EventInsurance.Domain.Enums;
using Moq;
using Xunit;

namespace EventInsurance.Test.Services
{
    public class ClaimServiceTests
    {
        private readonly Mock<IClaimRepository> _claimRepoMock;
        private readonly Mock<IActivePolicyRepository> _policyRepoMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly ClaimService _claimService;

        public ClaimServiceTests()
        {
            _claimRepoMock = new Mock<IClaimRepository>();
            _policyRepoMock = new Mock<IActivePolicyRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _userRepoMock = new Mock<IUserRepository>();

            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { new User { Id = 1, RoleId = 1 } });

            _claimService = new ClaimService(
                _claimRepoMock.Object,
                _policyRepoMock.Object,
                _notificationServiceMock.Object,
                _userRepoMock.Object);
        }

        [Fact]
        public async Task RaiseClaimAsync_ThrowsException_IfPolicyNotFound()
        {
            // Arrange
            var dto = new RaiseClaimRequestDto { PolicyId = 99 };
            _policyRepoMock.Setup(r => r.GetByIdAsync(dto.PolicyId))
                .ReturnsAsync((ActivePolicy?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _claimService.RaiseClaimAsync(dto));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task RaiseClaimAsync_ThrowsException_IfPolicyNotActive()
        {
            // Arrange
            var dto = new RaiseClaimRequestDto { PolicyId = 1 };
            var policy = new ActivePolicy { Id = 1, Status = PolicyStatus.Expired };
            _policyRepoMock.Setup(r => r.GetByIdAsync(dto.PolicyId))
                .ReturnsAsync(policy);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _claimService.RaiseClaimAsync(dto));
            Assert.Contains("Active policies", ex.Message);
        }

        [Fact]
        public async Task RaiseClaimAsync_CreatesClaim_AndNotifiesAdmin()
        {
            // Arrange
            var dto = new RaiseClaimRequestDto
            {
                PolicyId = 1,
                CustomerId = 2,
                ClaimAmount = 500,
                Description = "Accident"
            };
            var policy = new ActivePolicy { Id = 1, Status = PolicyStatus.Active, PolicyNumber = "POL1", Customer = new User{FullName="Bob"} };
            
            _policyRepoMock.Setup(r => r.GetByIdAsync(dto.PolicyId)).ReturnsAsync(policy);
            _claimRepoMock.Setup(r => r.AddAsync(It.IsAny<Claim>())).Returns(Task.CompletedTask);

            // Act
            var result = await _claimService.RaiseClaimAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.ClaimAmount, result.ClaimAmount);
            _claimRepoMock.Verify(r => r.AddAsync(It.Is<Claim>(c => c.ClaimAmount == dto.ClaimAmount)), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(1, It.IsAny<string>(), It.IsAny<string>()), Times.Once); // Notifies Admin
        }

        [Fact]
        public async Task ApproveClaimAsync_ThrowsException_IfClaimNotSubmittedOrReview()
        {
            // Arrange
            var claim = new Claim { Id = 1, Status = ClaimStatus.Settled };
            _claimRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(claim);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _claimService.ApproveClaimAsync(1, new ClaimReviewDto { ClaimsOfficerId = 5 }));
            Assert.Contains("cannot be approved", ex.Message);
        }

        [Fact]
        public async Task ApproveClaimAsync_UpdatesStatus_AndNotifiesCustomer()
        {
            // Arrange
            var claim = new Claim { Id = 1, Status = ClaimStatus.UnderReview, CustomerId = 10 };
            _claimRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(claim);
            _claimRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _claimService.ApproveClaimAsync(1, new ClaimReviewDto { ClaimsOfficerId = 5 });

            // Assert
            Assert.Equal("Approved", result.Status);
            _claimRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(claim.CustomerId, "Claim Approved", It.IsAny<string>()), Times.Once);
        }
    }
}
