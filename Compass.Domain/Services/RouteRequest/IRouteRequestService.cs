using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.RouteRequest
{
    public interface IRouteRequestService
    {
        Task<CompassResult> RouteRequest(CompassEvent compassEvent);
    }
}