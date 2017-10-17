using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.Subscription;
using Compass.Domain.Services.ValidateApplicationToken;
using Compass.Shared;
using Xunit;
using FakeItEasy;


namespace Compass.Domain.Tests.Services.Subscription
{
    
    public class SubscriptionServiceTests
    {
        private readonly ISubscriptionService _sut;
        private readonly IDataStore _dataStore;
        private readonly ICompassEnvironment _compassEnvironment;
        private readonly IReplayQueuedEventsService _replayQueuedEventsService;
        private readonly IValidateApplicationTokenService _validateApplicationTokenService;

        public SubscriptionServiceTests()
        {
            _dataStore = A.Fake<IDataStore>();
            _compassEnvironment = A.Fake<ICompassEnvironment>();
            _replayQueuedEventsService = A.Fake<IReplayQueuedEventsService>();
            _validateApplicationTokenService = A.Fake<IValidateApplicationTokenService>();
            _sut = new SubscriptionService(_dataStore, _compassEnvironment, _replayQueuedEventsService, _validateApplicationTokenService);
        }

        [Fact]
        public async Task Subscription_ValidateAppToken()
        {
            // Arrange
            var appToken = Guid.NewGuid();
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(appToken))
             .Returns(new RegisteredApplication());

            // Act
            await _sut.SubscribeAsync(new ServiceSubscription() { ApplicationToken = appToken });

            // Assert
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(appToken))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task Subscription_SaveTheSubscription()
        {
            // Arrange
            var appToken = Guid.NewGuid();
            var serviceSubscription = new ServiceSubscription() { ApplicationToken = appToken };
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(appToken))
             .Returns(new RegisteredApplication());
            var ttl = (uint)10000;
            A.CallTo(() => _compassEnvironment.GetSubscriptionTtl())
             .Returns(ttl);

            // Act
            await _sut.SubscribeAsync(serviceSubscription);

            // Assert
            A.CallTo(() => _dataStore.UpsertAsync(A<ServiceSubscription>.That.Matches(arg => arg.ApplicationToken == appToken), ttl))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _dataStore.UpsertAsync(serviceSubscription, ttl))
             .MustHaveHappened(Repeated.Exactly.Once); 
        }

        [Fact]
        public async Task Subscription_OverwriteExistingSubscription()
        {
            // Arrange
            var appToken = Guid.NewGuid();
            var serviceSubscription = new ServiceSubscription() { ApplicationToken = appToken };
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(appToken))
             .Returns(new RegisteredApplication());
            var ttl = (uint)10000;
			A.CallTo(() => _compassEnvironment.GetSubscriptionTtl())
			 .Returns(ttl);

            var serviceSubscriptionIdentifier = Guid.NewGuid();
            A.CallTo(() => _dataStore.GetServiceSubscriptionAsync(appToken))
             .Returns(new ServiceSubscription()
             {
                 Identifier = serviceSubscriptionIdentifier,
                 ApplicationToken = appToken
             });

            // Act
            await _sut.SubscribeAsync(serviceSubscription);

            // Assert
            A.CallTo(() => _dataStore.UpsertAsync(A<ServiceSubscription>.That.Matches(arg => arg.Identifier == serviceSubscriptionIdentifier), ttl))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _dataStore.UpsertAsync(serviceSubscription, ttl))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task Subscription_ReplayEvents()
        {
            // Arrange
            var serviceSubscription = new ServiceSubscription
            {
                SubscribedEvents = { "test", "test1" },
                ApplicationToken = Guid.NewGuid()
            };
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(serviceSubscription.ApplicationToken))
             .Returns(Task.FromResult(new RegisteredApplication()));

            // Act
            await _sut.SubscribeAsync(serviceSubscription);

            // Assert
            A.CallTo(() => _replayQueuedEventsService.ReplayQueuedEvents(serviceSubscription))
             .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
