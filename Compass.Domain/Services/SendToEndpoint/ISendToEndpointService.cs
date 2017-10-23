using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.SendToEndpoint
{
    public interface ISendToEndpointService
    {
        Task<IReadOnlyCollection<SendToEndpointResult>> SendToEndpointAsync(IReadOnlyCollection<ServiceSubscription> subscriptions, CompassEvent compassEvent);
    }
}
