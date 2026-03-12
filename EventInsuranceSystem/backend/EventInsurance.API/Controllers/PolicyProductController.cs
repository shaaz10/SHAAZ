using EventInsurance.Application.Interfaces.Repositories;
using EventInsurance.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyProductController : ControllerBase
    {
        private readonly IPolicyRepository _repository;

        public PolicyProductController(IPolicyRepository repository)
        {
            _repository = repository;
        }

        /// <summary>GET /api/policyproduct — public, returns active products</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>POST /api/policyproduct — Admin only, create new product</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PolicyProductDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Product name is required.");

            var product = new PolicyProduct
            {
                Name = dto.Name,
                Description = dto.Description ?? "",
                BasePremium = dto.BasePremium,
                CoverageAmount = dto.CoverageAmount,
                CreatedByUserId = GetUserId(),
                IsApprovedByAdmin = true,
                ApprovedByAdminId = GetUserId(),
                IsActive = true
            };

            await _repository.AddProductAsync(product);
            return Ok(new { message = "Product created successfully.", product });
        }

        /// <summary>PUT /api/policyproduct/{id} — Admin only, update existing product</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] PolicyProductDto dto)
        {
            if (dto == null) return BadRequest();
            var product = new PolicyProduct
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description ?? "",
                BasePremium = dto.BasePremium,
                CoverageAmount = dto.CoverageAmount
            };
            await _repository.UpdateProductAsync(product);
            return Ok(new { message = "Product updated successfully." });
        }

        /// <summary>DELETE /api/policyproduct/{id} — Admin only, soft-deletes a product</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteProductAsync(id);
            return Ok(new { message = "Product deactivated successfully." });
        }

        private int GetUserId()
        {
            var claim = User.FindFirst("sub") ?? User.FindFirst("nameid") ??
                        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }
    }

    public class PolicyProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePremium { get; set; }
        public decimal CoverageAmount { get; set; }
    }
}
