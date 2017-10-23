using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;

namespace Compass.Domain.Services.QueueEvent
{
    public class QueueEventService : IQueueEventService
    {
        private readonly IDataStore _dataStore;

        public QueueEventService(
            IDataStore dataStore
        )
        {
            _dataStore = dataStore;
        }
        public async Task QueueEventAsync(CompassEvent compassEvent)
        {
            compassEvent.Identifier = Guid.NewGuid();
            compassEvent.DateCreated = DateTime.Now;

            var queuedEvents = await GetQueuedEventsAsync();
            queuedEvents.Events.Add(compassEvent);

            await _dataStore.UpsertAsync(queuedEvents);
        }

        private async Task<QueuedEvents> GetQueuedEventsAsync()
        {
            return await _dataStore.GetQueuedEventsAsync() ??
                               new QueuedEvents {Identifier = Guid.NewGuid()};
        }

        public async Task DeQueueEventAsync(CompassEvent compassEvent)
        {
            var queuedEvents = await _dataStore.GetQueuedEventsAsync();
            queuedEvents.Events.Remove(compassEvent);

            await _dataStore.UpsertAsync(queuedEvents);
        }
    }
}
