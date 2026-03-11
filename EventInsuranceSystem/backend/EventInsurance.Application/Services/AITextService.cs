using EventInsurance.Application.Interfaces.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EventInsurance.Application.Services
{
    public class AITextService : IAITextService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AITextService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> EnhanceTextAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            using var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.PostAsJsonAsync("http://127.0.0.1:8000/enhance-text", new { text = text });
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<EnhanceTextResponse>();
                    return result?.enhanced_text ?? text;
                }
            }
            catch
            {
                // Fallback to original text if AI service is down
                return text;
            }

            return text;
        }

        private class EnhanceTextResponse
        {
            public string enhanced_text { get; set; } = string.Empty;
        }
    }
}
