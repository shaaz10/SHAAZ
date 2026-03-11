using EventInsurance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventInsurance.API.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AITextController : ControllerBase
    {
        private readonly IAITextService _aiTextService;

        public AITextController(IAITextService aiTextService)
        {
            _aiTextService = aiTextService;
        }

        [HttpPost("enhance-text")]
        public async Task<IActionResult> EnhanceText([FromBody] EnhanceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Text))
            {
                return BadRequest(new { message = "Text is required" });
            }

            var enhancedText = await _aiTextService.EnhanceTextAsync(request.Text);
            return Ok(new { enhancedText });
        }

        public class EnhanceRequest
        {
            public string Text { get; set; } = string.Empty;
        }
    }
}
