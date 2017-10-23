using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.GetServiceSubscriptionsForEvent;
using Compass.Domain.Services.KafkaStream;
using Compass.Domain.Services.RouteRequest;
using Compass.Domain.Services.SendToEndpoint;
using Compass.Domain.Services.ValidateApplicationToken;
using Xunit;
using FakeItEasy;

namespace Compass.Domain.Tests.Services.RouteRequest
{
    
    public class RouteRequestServiceTests
    {
        private readonly IRouteRequestService _sut;
        private readonly ISendToEndpointService _sendToEndpointService;
        private readonly IValidateApplicationTokenService _validateApplicationTokenService;
        private readonly IKafkaStreamService _kafkaStreamService;
        private readonly IGetServiceSubscriptionsForEventService _getServiceSubscriptionsForEventService;
        private readonly IDataStore _dataStore;

        public RouteRequestServiceTests()
        {
            _sendToEndpointService = A.Fake<ISendToEndpointService>();
            _validateApplicationTokenService = A.Fake<IValidateApplicationTokenService>();
            _kafkaStreamService = A.Fake<IKafkaStreamService>();
            _getServiceSubscriptionsForEventService = A.Fake<IGetServiceSubscriptionsForEventService>();
            _dataStore = A.Fake<IDataStore>();

            _sut = new RouteRequestService(
                _sendToEndpointService,
                _validateApplicationTokenService,
                _kafkaStreamService,
                _getServiceSubscriptionsForEventService,
                _dataStore
            );
        }

        [Fact]
        public async Task RouteRequest_ValidateAppToken()
        {
            // Arrange
            var appToken = Guid.NewGuid();
            var compassEvent = new CompassEvent() { EventName = "test", ApplicationToken = appToken };
            var subscriptions = new List<ServiceSubscription>
            {
                new ServiceSubscription()
            };
            A.CallTo(() => _getServiceSubscriptionsForEventService.GetServiceSubscriptionsAsync(compassEvent))
             .Returns(subscriptions);

            // Act
            await _sut.RouteRequest(compassEvent);

            // Assert
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(appToken))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task RouteRequest_SendToAllSubscriptions()
        {
            // Arrange
            var subscriptions = new List<ServiceSubscription>
            {
                new ServiceSubscription()
                {
                    ApplicationToken = Guid.NewGuid(),
                    ApplicationUri = new Uri("http://www.example.com/"),
                    SubscribedEvents = new List<string>() { "test"}
                },
                new ServiceSubscription()
                {
                    ApplicationToken = Guid.NewGuid(),
                    ApplicationUri = new Uri("http://www.example2.com/"),
                    SubscribedEvents = new List<string>() { "test"}
                }
            };

            var payload = new {test = "test"};
            var compassEvent = new CompassEvent
            {
                EventName = "test", 
                ApplicationToken = Guid.NewGuid(),
                Payload = payload
            };

            A.CallTo(() => _getServiceSubscriptionsForEventService.GetServiceSubscriptionsAsync(compassEvent))
             .Returns(subscriptions);

            // Act
            await _sut.RouteRequest(compassEvent);

            // Assert
            A.CallTo(() => _sendToEndpointService.SendToEndpointAsync(subscriptions, compassEvent))
             .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
