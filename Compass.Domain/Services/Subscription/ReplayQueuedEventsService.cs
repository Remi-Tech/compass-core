using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.QueueEvent;
using Compass.Domain.Services.SendToEndpoint;

namespace Compass.Domain.Services.Subscription
{
    public class ReplayQueuedEventsService : IReplayQueuedEventsService
    {
        private readonly IDataStore _dataStore;
        private readonly ISendToEndpointService _sendToEndpointService;
        private readonly IQueueEventService _queueEventService;

        public ReplayQueuedEventsService(
            IDataStore dataStore,
            ISendToEndpointService sendToEndpointService,
            IQueueEventService queueEventService
        )
        {
            _dataStore = dataStore;
            _sendToEndpointService = sendToEndpointService;
            _queueEventService = queueEventService;
        }
        public async Task ReplayQueuedEvents(ServiceSubscription serviceSubscription)
        {
            var queuedEvents = await _dataStore.GetQueuedEventsAsync();

            if (ThereAreNoEvents(queuedEvents))
            {
                return;
            }

            await ProcessQueuedEvents(serviceSubscription, queuedEvents);
        }

        private async Task ProcessQueuedEvents(ServiceSubscription serviceSubscription, QueuedEvents queuedEvents)
        {
            var eventsToReplay = await GetEventsToReplay(serviceSubscription, queuedEvents);

            var parallelRequests = new List<Task>();

            foreach (var compassEvent in eventsToReplay)
            {
                parallelRequests.Add(SendEvent(serviceSubscription, compassEvent));
            }

            await Task.WhenAll(parallelRequests);
        }

        private async Task<IEnumerable<CompassEvent>> GetEventsToReplay(ServiceSubscription serviceSubscription, QueuedEvents queuedEvents)
        {
            var registeredApplication =
                            await _dataStore.GetRegisteredApplicationAsync(serviceSubscription.ApplicationToken);

            var eventsToReplay = queuedEvents.Events.Where(
                queuedEvent => serviceSubscription.SubscribedEvents.Contains(queuedEvent.EventName)
                               || registeredApplication.LastEventsSubscribed.Contains(queuedEvent.EventName));
            return eventsToReplay;
        }

        private static bool ThereAreNoEvents(QueuedEvents queuedEvents)
        {
            return queuedEvents?.Events == null || queuedEvents.Events.Count < 1;
        }

        private async Task SendEvent(ServiceSubscription serviceSubscription, CompassEvent compassEvent)
        {
            await _sendToEndpointService.SendToEndpointAsync(new List<ServiceSubscription>()
            {
                serviceSubscription
            }, compassEvent.EventName, compassEvent.Payload);
            await _queueEventService.DeQueueEventAsync(compassEvent);
        }
    }
}
