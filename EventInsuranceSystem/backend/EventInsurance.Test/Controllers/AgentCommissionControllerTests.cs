using EventInsurance.API.Controllers;
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventInsurance.Test.Controllers
{
    public class AgentCommissionControllerTests
    {
        private Mock<IAgentCommissionService> _svcMock;
        private AgentCommissionController _controller;

        public AgentCommissionControllerTests()
        {
            _svcMock = new Mock<IAgentCommissionService>();
            _controller = new AgentCommissionController(_svcMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfCommissions()
        {
            // Arrange
            var list = new List<AgentCommissionResponseDto> { new AgentCommissionResponseDto { Id = 1 } };
            _svcMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

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
            var dto = new AgentCommissionResponseDto { Id = 1 };
            _svcMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }

        [Fact]
        public async Task GetByPolicy_ValidId_ReturnsOk()
        {
            // Arrange
            var dto = new AgentCommissionResponseDto { Id = 1, PolicyId = 10 };
            _svcMock.Setup(s => s.GetByPolicyIdAsync(10)).ReturnsAsync(dto);

            // Act
            var result = await _controller.GetByPolicy(10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }

        [Fact]
        public async Task GetByAgent_ValidId_ReturnsOk()
        {
            // Arrange
            var list = new List<AgentCommissionResponseDto> { new AgentCommissionResponseDto { Id = 1, AgentId = 3 } };
            _svcMock.Setup(s => s.GetByAgentIdAsync(3)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetByAgent(3);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GenerateForPolicy_ValidId_ReturnsOk()
        {
            // Arrange
            var dto = new AgentCommissionResponseDto { Id = 5, PolicyId = 7 };
            _svcMock.Setup(s => s.GenerateForPolicyAsync(7)).ReturnsAsync(dto);

            // Act
            var result = await _controller.GenerateForPolicy(7);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }
    }
}
