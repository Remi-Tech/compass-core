using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.ValidateApplicationToken;
using Compass.Shared;

namespace Compass.Domain.Services.Subscription
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IDataStore _dataStore;
        private readonly ICompassEnvironment _compassEnvironment;
        private readonly IReplayQueuedEventsService _replayQueuedEventsService;
        private readonly IValidateApplicationTokenService _validateApplicationTokenService;

        public SubscriptionService(
            IDataStore dataStore,
            ICompassEnvironment compassEnvironment,
            IReplayQueuedEventsService replayQueuedEventsService,
            IValidateApplicationTokenService validateApplicationTokenService
            )
        {
            _dataStore = dataStore;
            _compassEnvironment = compassEnvironment;
            _replayQueuedEventsService = replayQueuedEventsService;
            _validateApplicationTokenService = validateApplicationTokenService;
        }

        public async Task<ServiceSubscription> SubscribeAsync(ServiceSubscription serviceSubcription)
        {
            var registeredApplication =
                await _validateApplicationTokenService.ValidateApplicationTokenAsync(
                    serviceSubcription.ApplicationToken);

            var subscription = await SaveSubscriptionAsync(serviceSubcription);
            var updateTask = UpdateLastSeenAsync(registeredApplication);

            // TODO: Determine if we should replay events...
            // TODO: How do we prevent duplicate data while replaying events?
            // This could happen if Service A went down, and Service B, listening 
            // to the same event is still up when Service A comes back up. It will
            // receive the the event a second time.
            var replayTask = _replayQueuedEventsService.ReplayQueuedEvents(serviceSubcription);

            await Task.WhenAll(new List<Task> { replayTask, updateTask });

            return subscription;
        }

        private async Task<ServiceSubscription> SaveSubscriptionAsync(ServiceSubscription serviceSubscription)
        {
            serviceSubscription.Identifier = await GetIdentifierAsync(serviceSubscription);
            uint subscriptionTtl = _compassEnvironment.GetSubscriptionTtl();
            await _dataStore.UpsertAsync(serviceSubscription, subscriptionTtl);
            return serviceSubscription;
        }

        private async Task<Guid> GetIdentifierAsync(ServiceSubscription serviceSubscription)
        {
            // When saving a service subscription, we must first check to see if one exists already.
            // If it does, we need to use the existing documents identifier in order to overwrite it.
            var existingSubscription = await _dataStore.GetServiceSubscriptionAsync(serviceSubscription.ApplicationToken);
            return existingSubscription?.Identifier ?? Guid.NewGuid();
        }

        private async Task UpdateLastSeenAsync(RegisteredApplication registeredApplication)
        {
            registeredApplication.LastSeen = DateTime.UtcNow;
            await _dataStore.UpsertAsync(registeredApplication);
        }
    }
}
