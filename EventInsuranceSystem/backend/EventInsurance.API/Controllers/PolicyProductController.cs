using EventInsurance.Application.Interfaces.Repositories;
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

        /// <summary>
        /// Retrieves the list of all available insurance products.
        /// Publicly accessible to allow potential customers to see coverage options.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Fetches all defined policy products from the database repository
            var products = await _repository.GetAllProductsAsync();

            // Returns the list of products for display in the UI
            return Ok(products);
        }
    }
}
