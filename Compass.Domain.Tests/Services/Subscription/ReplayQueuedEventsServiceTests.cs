using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.QueueEvent;
using Compass.Domain.Services.SendToEndpoint;
using Compass.Domain.Services.Subscription;
using Xunit;
using FakeItEasy;

namespace Compass.Domain.Tests.Services.Subscription
{

    public class ReplayQueuedEventsServiceTests
    {
        private readonly IReplayQueuedEventsService _sut;
        private readonly IDataStore _dataStore;
        private readonly ISendToEndpointService _sendToEndpointService;
        private readonly IQueueEventService _queueEventService;

        public ReplayQueuedEventsServiceTests()
        {
            _dataStore = A.Fake<IDataStore>();
            _sendToEndpointService = A.Fake<ISendToEndpointService>();
            _queueEventService = A.Fake<IQueueEventService>();
            _sut = new ReplayQueuedEventsService(_dataStore, _sendToEndpointService, _queueEventService);
        }

        [Fact]
        public async Task ReplayQueuedEvents_SendMultipleEvents()
        {

            // Arrange
            var serviceSubscription = new ServiceSubscription
            {
                ApplicationUri = new Uri("http://www.example.com/"),
                SubscribedEvents = new List<string>() { "test", "test2" }
            };

            var compassEvents = new List<CompassEvent>()
            {
                new CompassEvent
                {
                    ApplicationToken = Guid.NewGuid(),
                    EventName = "test",
                    Identifier = Guid.NewGuid(),
                    Payload = new {test = "test"}
                },
                new CompassEvent
                {
                    ApplicationToken = Guid.NewGuid(),
                    EventName = "test2",
                    Identifier = Guid.NewGuid(),
                    Payload = new {test2 = "test"}
                }
            };

            var queuedEvent = new QueuedEvents { Events = compassEvents };
            A.CallTo(() => _dataStore.GetQueuedEventsAsync())
             .Returns(queuedEvent);
            A.CallTo(() => _dataStore.GetRegisteredApplicationAsync(serviceSubscription.ApplicationToken))
             .Returns(Task.FromResult(
                 new RegisteredApplication() { LastEventsSubscribed = new List<string>() { "test" } }));

            // Act
            await _sut.ReplayQueuedEvents(serviceSubscription);

            // Assert
            A.CallTo(() => _sendToEndpointService.SendToEndpointAsync(
                    A<List<ServiceSubscription>>.That.IsSameSequenceAs(
                        new List<ServiceSubscription> { serviceSubscription }), compassEvents[0].EventName, compassEvents[0].Payload))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => _sendToEndpointService.SendToEndpointAsync(
                    A<List<ServiceSubscription>>.That.IsSameSequenceAs(
                        new List<ServiceSubscription> { serviceSubscription }), compassEvents[1].EventName, compassEvents[1].Payload))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task ReplayQueuedEvents_OnlySendSuppliedEvents()
        {

            // Arrange
            var serviceSubscription = new ServiceSubscription
            {
                ApplicationUri = new Uri("http://www.example.com/"),
                SubscribedEvents = new List<string>() { "test" },
                ApplicationToken = Guid.NewGuid()
            };

            var compassEvents = new List<CompassEvent>()
            {
                new CompassEvent
                {
                    ApplicationToken = Guid.NewGuid(),
                    EventName = "test",
                    Identifier = Guid.NewGuid(),
                    Payload = new {test = "test"}
                },
                new CompassEvent
                {
                    ApplicationToken = Guid.NewGuid(),
                    EventName = "test2",
                    Identifier = Guid.NewGuid(),
                    Payload = new {test2 = "test"}
                }
            };

            var queuedEvent = new QueuedEvents { Events = compassEvents };
            A.CallTo(() => _dataStore.GetQueuedEventsAsync())
             .Returns(queuedEvent);
            A.CallTo(() => _dataStore.GetRegisteredApplicationAsync(serviceSubscription.ApplicationToken))
             .Returns(Task.FromResult(
                 new RegisteredApplication() { LastEventsSubscribed = new List<string>() { "test" } }));

            // Act
            await _sut.ReplayQueuedEvents(serviceSubscription);

            // Assert
            A.CallTo(() => _sendToEndpointService.SendToEndpointAsync(
                    A<List<ServiceSubscription>>.That.IsSameSequenceAs(
                        new List<ServiceSubscription> { serviceSubscription }), compassEvents[0].EventName, compassEvents[0].Payload))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => _sendToEndpointService.SendToEndpointAsync(
                    A<List<ServiceSubscription>>.That.IsSameSequenceAs(
                        new List<ServiceSubscription> { serviceSubscription }), compassEvents[1].EventName, compassEvents[1].Payload))
                        .MustNotHaveHappened();
        }
    }
}
