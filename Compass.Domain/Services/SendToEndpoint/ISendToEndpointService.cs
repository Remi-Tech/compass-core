using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.RouteRequest.SendToEndpoint
{
    public interface ISendToEndpointService
    {
        Task<dynamic[]> SendToEndpointAsync(IReadOnlyCollection<ServiceSubscription> subscriptions, dynamic payload);
    }
}
