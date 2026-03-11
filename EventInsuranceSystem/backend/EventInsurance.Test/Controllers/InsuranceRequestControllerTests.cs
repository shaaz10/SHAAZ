using System.Security.Claims;
using EventInsurance.API.Controllers;
using EventInsurance.Application.DTOs;
using EventInsurance.Application.Interfaces.Services;
using EventInsurance.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventInsurance.Test.Controllers
{
    public class InsuranceRequestControllerTests
    {
        private Mock<IInsuranceRequestService> _svcMock;
        private InsuranceRequestController _controller;

        public InsuranceRequestControllerTests()
        {
            _svcMock = new Mock<IInsuranceRequestService>();
            _controller = new InsuranceRequestController(_svcMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new System.Security.Claims.Claim[] {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "5")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task Create_ValidData_ReturnsOk()
        {
            // Arrange
            var dto = new CreateInsuranceRequestDto { EventType = "Wedding", EventLocation = "Venue" };
            _svcMock.Setup(s => s.CreateRequestAsync(5, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AssignAgent_AdminAction_ReturnsOk()
        {
            // Arrange
            var dto = new AssignAgentDto { RequestId = 1, AgentId = 3 };
            _svcMock.Setup(s => s.AssignAgentAsync(1, 3)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignAgent(dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetUnassignedRequests_AdminAction_ReturnsOk()
        {
            // Arrange
            var list = new List<InsuranceRequest> { new InsuranceRequest { Id = 1 } };
            _svcMock.Setup(s => s.GetUnassignedRequestsAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetUnassignedRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetAssignedRequests_AgentAction_ReturnsOk()
        {
            // Arrange
            var list = new List<InsuranceRequest> { new InsuranceRequest { Id = 1, AssignedAgentId = 5 } };
            _svcMock.Setup(s => s.GetAssignedRequestsAsync(5)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetAssignedRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetCustomerRequests_CustomerAction_ReturnsOk()
        {
            // Arrange
            var list = new List<InsuranceRequest> { new InsuranceRequest { Id = 1, CustomerId = 5 } };
            _svcMock.Setup(s => s.GetCustomerRequestsAsync(5)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetCustomerRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }
    }
}
