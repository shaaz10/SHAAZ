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
    public class AdminApprovalServiceTests
    {
        private readonly Mock<IPolicyApplicationRepository> _appRepoMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IActivePolicyService> _activePolicyServiceMock;
        private readonly AdminApprovalService _adminApprovalService;

        public AdminApprovalServiceTests()
        {
            _appRepoMock = new Mock<IPolicyApplicationRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _activePolicyServiceMock = new Mock<IActivePolicyService>();

            _adminApprovalService = new AdminApprovalService(
                _appRepoMock.Object,
                _notificationServiceMock.Object,
                _activePolicyServiceMock.Object);
        }

        [Fact]
        public async Task ApproveApplicationAsync_ThrowsException_IfAppNotFound()
        {
            // Arrange
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PolicyApplication?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _adminApprovalService.ApproveApplicationAsync(1, new AdminApprovalRequestDto()));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task ApproveApplicationAsync_ThrowsException_IfStatusNotEscalated()
        {
            // Arrange
            var app = new PolicyApplication { Id = 1, Status = PolicyApplicationStatus.UnderReview };
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _adminApprovalService.ApproveApplicationAsync(1, new AdminApprovalRequestDto()));
            Assert.Contains("Only escalated applications", ex.Message);
        }

        [Fact]
        public async Task ApproveApplicationAsync_UpdatesStatus_AndNotifies()
        {
            // Arrange
            var app = new PolicyApplication 
            { 
                Id = 1, 
                Status = PolicyApplicationStatus.EscalatedToAdmin,
                AgentId = 5,
                CustomerId = 10
            };
            var dto = new AdminApprovalRequestDto { ApprovalNotes = "Looks good" };
            
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);
            _appRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _adminApprovalService.ApproveApplicationAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsApproved);
            Assert.Equal("Approved", result.Status);
            _appRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(app.AgentId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(app.CustomerId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RejectApplicationAsync_ThrowsException_IfAppNotFound()
        {
            // Arrange
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PolicyApplication?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _adminApprovalService.RejectApplicationAsync(1, "reason"));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task RejectApplicationAsync_UpdatesStatusToRejected_AndNotifies()
        {
            // Arrange
            var app = new PolicyApplication 
            { 
                Id = 1, 
                Status = PolicyApplicationStatus.EscalatedToAdmin,
                AgentId = 5,
                CustomerId = 10
            };
            
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);
            _appRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _adminApprovalService.RejectApplicationAsync(1, "Missing docs");

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsApproved);
            Assert.Equal("Rejected", result.Status);
            _appRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(app.AgentId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(app.CustomerId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
