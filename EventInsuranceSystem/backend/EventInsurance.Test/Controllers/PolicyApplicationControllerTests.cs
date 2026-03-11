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
    public class PolicyApplicationControllerTests
    {
        private Mock<IPolicyApplicationService> _svcMock;
        private PolicyApplicationController _controller;

        public PolicyApplicationControllerTests()
        {
            _svcMock = new Mock<IPolicyApplicationService>();
            _controller = new PolicyApplicationController(_svcMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new System.Security.Claims.Claim[] {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "5")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task SelectPolicy_ValidSuggestion_ReturnsOk()
        {
            // Arrange
            var req = new SelectPolicyRequest(15);
            _svcMock.Setup(s => s.CreateApplicationAsync(req.SuggestionId, 5)).ReturnsAsync(101);

            // Act
            var result = await _controller.SelectPolicy(req);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetByAgent_AgentAction_ReturnsOk()
        {
            // Arrange
            var list = new List<PolicyApplication> { new PolicyApplication { Id = 1, AgentId = 5 } };
            _svcMock.Setup(s => s.GetApplicationsByAgentIdAsync(5)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetByAgent();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetByCustomer_CustomerIdParam_ReturnsOk()
        {
            // Arrange
            var list = new List<PolicyApplication> { new PolicyApplication { Id = 1, CustomerId = 10 } };
            _svcMock.Setup(s => s.GetApplicationsByCustomerIdAsync(10)).ReturnsAsync(list);

            // Act
            var result = await _controller.GetByCustomer(10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, okResult.Value);
        }
    }
}
