using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.RegisterNewApplication
{
    public interface IRegisterNewApplicationService
    {
        Task<RegisteredApplication> RegisterNewApplicationAsync(string applicationName);
    }
}
