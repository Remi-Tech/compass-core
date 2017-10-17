using Compass.Domain.Models;
using Polly;

namespace Compass.Domain.Services.GetServiceSubscriptionsForEvent
{
    public interface IGetServiceSubscriptionsForEventPolicy
    {
        Policy GetPolicy(CompassEvent compassEvent);
    }
}
