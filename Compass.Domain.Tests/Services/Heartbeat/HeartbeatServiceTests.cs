using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.Heartbeat;
using Compass.Domain.Services.ValidateApplicationToken;
using Compass.Shared;
using Xunit;
using FakeItEasy;

namespace Compass.Domain.Tests.Services.Heartbeat
{

    public class HeartbeatServiceTests
    {
        private readonly IHeartbeatService _sut;
        private readonly IValidateApplicationTokenService _validateApplicationTokenService;
        private readonly IDataStore _dataStore;
        private readonly ICompassEnvironment _compassEnvironment;

        public HeartbeatServiceTests()
        {
            _validateApplicationTokenService = A.Fake<IValidateApplicationTokenService>();
            _dataStore = A.Fake<IDataStore>();
            _compassEnvironment = A.Fake<ICompassEnvironment>();
            _sut = new HeartbeatService(_validateApplicationTokenService, _dataStore, _compassEnvironment);
        }

        [Fact]
        public async Task Heartbeat_ValidateAppToken()
        {
            // Arrange
            var serviceSubscription = new ServiceSubscription
            {
                ApplicationToken = Guid.NewGuid()
            };
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(serviceSubscription.ApplicationToken))
             .Returns(Task.FromResult(new RegisteredApplication()));

            // Act
            await _sut.Thump(serviceSubscription);

            // Assert
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(serviceSubscription.ApplicationToken))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task Heartbeat_UpdateRegisteredApplication()
        {
            // Arrange
            var serviceSubscription = new ServiceSubscription
            {
                ApplicationToken = Guid.NewGuid()
            };
            var registeredApplication = new RegisteredApplication();
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(serviceSubscription.ApplicationToken))
             .Returns(Task.FromResult(registeredApplication));

            // Act
            await _sut.Thump(serviceSubscription);

            // Assert
            A.CallTo(() => _dataStore.UpsertAsync(registeredApplication))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task Heartbeat_UpdateServiceSubscription()
        {
            // Arrange
            const uint ttl = 10000;
            A.CallTo(() => _compassEnvironment.GetSubscriptionTtl()).Returns(ttl);

            var serviceSubscription = new ServiceSubscription
            {
                ApplicationToken = Guid.NewGuid()
            };

            var registeredApplication = new RegisteredApplication();
            A.CallTo(() => _validateApplicationTokenService.ValidateApplicationTokenAsync(serviceSubscription.ApplicationToken))
             .Returns(Task.FromResult(registeredApplication));

			A.CallTo(() => _dataStore.GetServiceSubscriptionAsync(serviceSubscription.ApplicationToken))
             .Returns(Task.FromResult(serviceSubscription));
			
            // Act
            await _sut.Thump(serviceSubscription);

            // Assert
            A.CallTo(() => _dataStore.UpsertAsync(serviceSubscription, ttl))
             .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
