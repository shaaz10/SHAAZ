using System.Threading.Tasks;

namespace EventInsurance.Application.Interfaces.Services
{
    public interface IAITextService
    {
        Task<string> EnhanceTextAsync(string text);
    }
}
