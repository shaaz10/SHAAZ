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
    public class ActivePolicyServiceTests
    {
        private readonly Mock<IActivePolicyRepository> _activePolicyRepoMock;
        private readonly Mock<IPolicyApplicationRepository> _appRepoMock;
        private readonly Mock<IAgentCommissionRepository> _commissionRepoMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly ActivePolicyService _policyService;

        public ActivePolicyServiceTests()
        {
            _activePolicyRepoMock = new Mock<IActivePolicyRepository>();
            _appRepoMock = new Mock<IPolicyApplicationRepository>();
            _commissionRepoMock = new Mock<IAgentCommissionRepository>();
            _notificationServiceMock = new Mock<INotificationService>();

            _policyService = new ActivePolicyService(
                _activePolicyRepoMock.Object,
                _appRepoMock.Object,
                _commissionRepoMock.Object,
                _notificationServiceMock.Object);
        }

        [Fact]
        public async Task CreateActivePolicyAsync_ThrowsException_IfAppNotFound()
        {
            // Arrange
            _appRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync((PolicyApplication?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _policyService.CreateActivePolicyAsync(1));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task CreateActivePolicyAsync_ThrowsException_IfAppNotApproved()
        {
            // Arrange
            var app = new PolicyApplication { Id = 1, Status = PolicyApplicationStatus.PendingValidation };
            _appRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(app);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _policyService.CreateActivePolicyAsync(1));
            Assert.Contains("Only 'Approved'", ex.Message);
        }

        [Fact]
        public async Task CreateActivePolicyAsync_ThrowsException_IfPolicyAlreadyExists()
        {
            // Arrange
            var app = new PolicyApplication { Id = 1, Status = PolicyApplicationStatus.Approved };
            var existingPolicy = new ActivePolicy { Id = 10, PolicyNumber = "EI-123" };
            
            _appRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(app);
            _activePolicyRepoMock.Setup(r => r.GetByApplicationIdAsync(1)).ReturnsAsync(existingPolicy);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _policyService.CreateActivePolicyAsync(1));
            Assert.Contains("already exists", ex.Message);
        }

        [Fact]
        public async Task CreateActivePolicyAsync_CreatesPolicy_CreatesCommission_AndNotifies()
        {
            // Arrange
            var app = new PolicyApplication 
            { 
                Id = 1, 
                Status = PolicyApplicationStatus.Approved,
                PremiumAmount = 1000,
                AgentId = 5,
                CustomerId = 10
            };
            
            _appRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(app);
            _activePolicyRepoMock.Setup(r => r.GetByApplicationIdAsync(1)).ReturnsAsync((ActivePolicy?)null);
            _activePolicyRepoMock.Setup(r => r.AddAsync(It.IsAny<ActivePolicy>())).Returns(Task.CompletedTask);
            _commissionRepoMock.Setup(r => r.AddAsync(It.IsAny<AgentCommission>())).Returns(Task.CompletedTask);

            // Act
            var result = await _policyService.CreateActivePolicyAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Active", result.Status);
            _activePolicyRepoMock.Verify(r => r.AddAsync(It.IsAny<ActivePolicy>()), Times.Once);
            _commissionRepoMock.Verify(r => r.AddAsync(It.Is<AgentCommission>(c => c.CommissionAmount == 100)), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(app.CustomerId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(app.AgentId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetByAgentIdAsync_ReturnsMappedDtos()
        {
            // Arrange
            var policies = new List<ActivePolicy> 
            { 
                new ActivePolicy { Id = 1, PolicyNumber = "POL123" } 
            };
            _activePolicyRepoMock.Setup(r => r.GetByAgentIdAsync(5)).ReturnsAsync(policies);

            // Act
            var result = await _policyService.GetByAgentIdAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("POL123", result.First().PolicyNumber);
        }
    }
}
