using EventInsurance.API.Controllers;
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventInsurance.Test.Controllers
{
    public class AdminApprovalControllerTests
    {
        private readonly Mock<IAdminApprovalService> _serviceMock;
        private readonly Mock<IPolicyApplicationRepository> _appRepoMock;
        private readonly AdminApprovalController _controller;

        public AdminApprovalControllerTests()
        {
            _serviceMock = new Mock<IAdminApprovalService>();
            _appRepoMock = new Mock<IPolicyApplicationRepository>();
            _controller = new AdminApprovalController(_serviceMock.Object, _appRepoMock.Object);
        }

        [Fact]
        public async Task GetEscalated_ReturnsListOfApplications()
        {
            // Arrange
            var list = new List<PolicyApplication> { new PolicyApplication { Id = 1 } };
            _appRepoMock.Setup(r => r.GetEscalatedAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetEscalated();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetApproved_ReturnsListOfApplications()
        {
            // Arrange
            var list = new List<PolicyApplication> { new PolicyApplication { Id = 1 } };
            _appRepoMock.Setup(r => r.GetApprovedAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetApproved();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task ApproveApplication_ValidData_ReturnsOk()
        {
            // Arrange
            var dto = new AdminApprovalRequestDto { IsApproved = true, ApprovalNotes = "Approving" };
            var response = new AdminApprovalResponseDto { ApplicationId = 1, IsApproved = true };
            _serviceMock.Setup(s => s.ApproveApplicationAsync(1, dto)).ReturnsAsync(response);

            // Act
            var result = await _controller.ApproveApplication(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task RejectApplication_ValidData_ReturnsOk()
        {
            // Arrange
            var req = new RejectRequestDto("Risk too high");
            var response = new AdminApprovalResponseDto { ApplicationId = 1, IsApproved = false };
            _serviceMock.Setup(s => s.RejectApplicationAsync(1, req.RejectionReason)).ReturnsAsync(response);

            // Act
            var result = await _controller.RejectApplication(1, req);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task GetEscalated_EmptyList_ReturnsOk()
        {
            // Arrange
            _appRepoMock.Setup(r => r.GetEscalatedAsync()).ReturnsAsync(new List<PolicyApplication>());

            // Act
            var result = await _controller.GetEscalated();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Empty((IEnumerable<PolicyApplication>)okResult.Value!);
        }
    }
}
