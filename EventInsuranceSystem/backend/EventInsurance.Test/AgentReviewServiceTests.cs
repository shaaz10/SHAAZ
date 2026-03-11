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
    public class AgentReviewServiceTests
    {
        private readonly Mock<IPolicyApplicationRepository> _appRepoMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly AgentReviewService _agentReviewService;

        public AgentReviewServiceTests()
        {
            _appRepoMock = new Mock<IPolicyApplicationRepository>();
            _notificationServiceMock = new Mock<INotificationService>();

            _agentReviewService = new AgentReviewService(
                _appRepoMock.Object,
                _notificationServiceMock.Object);
        }

        [Fact]
        public async Task ReviewApplicationAsync_ThrowsException_IfApplicationNotFound()
        {
            // Arrange
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PolicyApplication?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _agentReviewService.ReviewApplicationAsync(1, 5, new AgentReviewApplicationDto()));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task ReviewApplicationAsync_ThrowsException_IfAgentNotAssigned()
        {
            // Arrange
            var app = new PolicyApplication { Id = 1, AgentId = 10 };
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _agentReviewService.ReviewApplicationAsync(1, 5, new AgentReviewApplicationDto()));
            Assert.Contains("Unauthorized", ex.Message);
        }

        [Fact]
        public async Task ReviewApplicationAsync_ThrowsException_IfStatusNotReviewable()
        {
            // Arrange
            var app = new PolicyApplication { Id = 1, AgentId = 5, Status = PolicyApplicationStatus.Approved };
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _agentReviewService.ReviewApplicationAsync(1, 5, new AgentReviewApplicationDto()));
            Assert.Contains("cannot be reviewed", ex.Message);
        }

        [Fact]
        public async Task ReviewApplicationAsync_ApprovesApplication_WhenIsApprovedIsTrue()
        {
            // Arrange
            var app = new PolicyApplication { Id = 1, AgentId = 5, CustomerId = 10, Status = PolicyApplicationStatus.UnderReview };
            var dto = new AgentReviewApplicationDto { IsApproved = true, ReviewNotes = "Good" };
            
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);
            _appRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _agentReviewService.ReviewApplicationAsync(1, 5, dto);

            // Assert
            Assert.Equal("Approved", result.Status);
            _appRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(app.CustomerId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task EscalateToAdminAsync_UpdatesStatusToEscalated_AndCreatesNotifications()
        {
            // Arrange
            var app = new PolicyApplication { Id = 1, AgentId = 5, CustomerId = 10, Status = PolicyApplicationStatus.UnderReview };
            
            _appRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);
            _appRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _agentReviewService.EscalateToAdminAsync(1, 5, "Needs admin look");

            // Assert
            Assert.True(result.IsEscalated);
            Assert.Equal("EscalatedToAdmin", result.Status);
            _appRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(n => n.CreateNotificationAsync(app.CustomerId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
