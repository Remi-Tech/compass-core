using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.QueueEvent;
using Xunit;
using FakeItEasy;

namespace Compass.Domain.Tests.Services.QueueEvent
{
    
    public class QueueEventServiceTests
    {
        private readonly IQueueEventService _sut;
        private readonly IDataStore _dataStore;

        public QueueEventServiceTests()
        {
            _dataStore = A.Fake<IDataStore>();
            _sut = new QueueEventService(_dataStore);
        }

        [Fact]
        public async Task QueueEvent_Queue()
        {
            // Arrange
            var compassEvent = new CompassEvent
            {
                EventName = "test"
            };

            var queuedEvents = new QueuedEvents()
            {
                Events = new List<CompassEvent>() {compassEvent}
            };

            A.CallTo(() => _dataStore.GetQueuedEventsAsync())
             .Returns(Task.FromResult(queuedEvents));
            
            // Act
            await _sut.QueueEventAsync(compassEvent);

            // Assert
            A.CallTo(() => _dataStore.UpsertAsync(queuedEvents)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _dataStore.UpsertAsync(A<QueuedEvents>.That.Matches(arg => arg.Events.Contains(compassEvent))))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task QueueEvent_DeQueue()
        {
            // Arrange
            var compassEvent = new CompassEvent
            {
                EventName = "test"
            };

            var queuedEvents = new QueuedEvents()
            {
                Events = new List<CompassEvent>() { compassEvent }
            };

            A.CallTo(() => _dataStore.GetQueuedEventsAsync())
             .Returns(Task.FromResult(queuedEvents));

            // Act
            await _sut.DeQueueEventAsync(compassEvent);

            // Assert
            A.CallTo(() => _dataStore.UpsertAsync(queuedEvents))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _dataStore.UpsertAsync(A<QueuedEvents>.That.Matches(arg => !arg.Events.Contains(compassEvent))))
             .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
