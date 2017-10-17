using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Exceptions;
using Compass.Domain.Models;

namespace Compass.Domain.Services.GetServiceSubscriptionsForEvent
{
    public class GetServiceSubscriptionsForEventService : IGetServiceSubscriptionsForEventService
    {
        private readonly IDataStore _dataStore;
        private readonly IGetServiceSubscriptionsForEventPolicy _getServiceSubscriptionsForEventPolicy;

        public GetServiceSubscriptionsForEventService(
            IDataStore dataStore,
            IGetServiceSubscriptionsForEventPolicy getServiceSubscriptionsForEventPolicy
            )
        {
            _dataStore = dataStore;
            _getServiceSubscriptionsForEventPolicy = getServiceSubscriptionsForEventPolicy;
        }

        public async Task<IReadOnlyCollection<ServiceSubscription>> GetServiceSubscriptionsAsync(CompassEvent compassEvent)
        {
            return await _getServiceSubscriptionsForEventPolicy.GetPolicy(compassEvent)
                .ExecuteAsync(() => GetSubscriptionsAsync(compassEvent));
        }

        private async Task<IReadOnlyCollection<ServiceSubscription>> GetSubscriptionsAsync(CompassEvent compassEvent)
        {
            var subscriptions = await _dataStore.GetServiceSubscribedToEventAsync(compassEvent.EventName);

            // TODO: Determine how to handle one service going down,
            // but another still alive
            if (subscriptions == null || subscriptions.Count == 0)
            {
                throw new NoValidSubscriptionsException(compassEvent);
            }

            return subscriptions.ToList();
        }
    }
}
