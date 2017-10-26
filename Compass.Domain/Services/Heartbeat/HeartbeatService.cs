using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.ValidateApplicationToken;
using Compass.Shared;

namespace Compass.Domain.Services.Heartbeat
{
    public class HeartbeatService : IHeartbeatService
    {
        private readonly IValidateApplicationTokenService _validateApplicationTokenService;
        private readonly IDataStore _dataStore;
        private readonly ICompassEnvironment _compassEnvironment;

        public HeartbeatService(
            IValidateApplicationTokenService validateApplicationTokenService,
            IDataStore dataStore,
            ICompassEnvironment compassEnvironment
            )
        {
            _validateApplicationTokenService = validateApplicationTokenService;
            _dataStore = dataStore;
            _compassEnvironment = compassEnvironment;
        }

        public async Task Thump(ServiceSubscription serviceSubscription)
        {
            var registeredApplication = await _validateApplicationTokenService.ValidateApplicationTokenAsync(serviceSubscription.ApplicationToken);
            registeredApplication.LastSeen = DateTime.UtcNow;
            await _dataStore.UpsertAsync(registeredApplication);
            var subscription = await GetServiceSubscription(serviceSubscription);
            var subscriptionTtl = _compassEnvironment.GetSubscriptionTtl();
            await _dataStore.UpsertAsync(subscription, subscriptionTtl);
        }

        private async Task<ServiceSubscription> GetServiceSubscription(ServiceSubscription serviceSubscription)
        {
            var subscription = await _dataStore.GetServiceSubscriptionAsync(serviceSubscription.ApplicationToken);
            if (subscription == null)
            {
                subscription = serviceSubscription;
                subscription.Identifier = Guid.NewGuid();
            }

            return subscription;
        }
    }
}
