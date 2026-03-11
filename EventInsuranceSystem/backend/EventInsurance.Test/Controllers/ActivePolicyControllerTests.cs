using EventInsurance.API.Controllers;
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventInsurance.Test.Controllers
{
    public class ActivePolicyControllerTests
    {
        private Mock<IActivePolicyService> _activePolicySvcMock;
        private ActivePolicyController _controller;

        public ActivePolicyControllerTests()
        {
            _activePolicySvcMock = new Mock<IActivePolicyService>();
            _controller = new ActivePolicyController(_activePolicySvcMock.Object);
        }

        [Fact]
        public async Task CreateActivePolicy_ValidAppId_ReturnsOk()
        {
            // Arrange
            var dto = new ActivePolicyResponseDto { Id = 10, PolicyNumber = "POL123" };
            _activePolicySvcMock.Setup(s => s.CreateActivePolicyAsync(1)).ReturnsAsync(dto);

            // Act
            var result = await _controller.CreateActivePolicy(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsCompleteList()
        {
            // Arrange
            var list = new List<ActivePolicyResponseDto> { new ActivePolicyResponseDto { Id = 1 } };
            _activePolicySvcMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetByCustomer_ReturnsFilteredResult()
        {
            // Arrange
            var list = new List<ActivePolicyResponseDto> { new ActivePolicyResponseDto { Id = 1, CustomerId = 5 } };
            _activePolicySvcMock.Setup(s => s.GetByCustomerIdAsync(5)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetByCustomer(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetByAgent_ReturnsFilteredResult()
        {
            // Arrange
            var list = new List<ActivePolicyResponseDto> { new ActivePolicyResponseDto { Id = 1, AgentId = 3 } };
            _activePolicySvcMock.Setup(s => s.GetByAgentIdAsync(3)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetByAgent(3);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsOk()
        {
            // Arrange
            var dto = new ActivePolicyResponseDto { Id = 1 };
            _activePolicySvcMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }
    }
}
