using EventInsurance.API.Controllers;
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventInsurance.Test.Controllers
{
    public class ClaimControllerTests
    {
        private readonly Mock<IClaimService> _claimSvcMock;
        private readonly ClaimController _controller;

        public ClaimControllerTests()
        {
            _claimSvcMock = new Mock<IClaimService>();
            _controller = new ClaimController(_claimSvcMock.Object);
        }

        [Fact]
        public async Task RaiseClaim_ValidData_ReturnsOk()
        {
            // Arrange
            var dto = new RaiseClaimRequestDto { PolicyId = 1, ClaimAmount = 5000 };
            _claimSvcMock.Setup(s => s.RaiseClaimAsync(dto)).ReturnsAsync(new ClaimResponseDto { Id = 101, Status = "Submitted" });

            // Act
            var result = await _controller.RaiseClaim(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfClaims()
        {
            // Arrange
            var list = new List<ClaimResponseDto> { new ClaimResponseDto { Id = 1, Status = "Pending" } };
            _claimSvcMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsOk()
        {
            // Arrange
            var dto = new ClaimResponseDto { Id = 1, Status = "Pending" };
            _claimSvcMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }

        [Fact]
        public async Task ApproveClaim_ValidData_ReturnsOk()
        {
            // Arrange
            var dto = new ClaimReviewDto { ClaimsOfficerId = 5, Notes = "Approved" };
            _claimSvcMock.Setup(s => s.ApproveClaimAsync(1, dto)).ReturnsAsync(new ClaimResponseDto { Id = 1, Status = "Approved" });

            // Act
            var result = await _controller.ApproveClaim(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task SettleClaim_ValidData_ReturnsOk()
        {
            // Arrange
            var dto = new ClaimSettlementDto { ClaimsOfficerId = 5, SettlementNotes = "Settle" };
            _claimSvcMock.Setup(s => s.SettleClaimAsync(1, dto)).ReturnsAsync(new ClaimResponseDto { Id = 1, Status = "Settled" });

            // Act
            var result = await _controller.SettleClaim(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
