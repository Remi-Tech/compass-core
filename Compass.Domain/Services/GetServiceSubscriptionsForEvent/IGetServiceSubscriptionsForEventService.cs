using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.GetServiceSubscriptionsForEvent
{
    public interface IGetServiceSubscriptionsForEventService
    {
        Task<IReadOnlyCollection<ServiceSubscription>> GetServiceSubscriptionsAsync(CompassEvent compassEvent);
    }
}
