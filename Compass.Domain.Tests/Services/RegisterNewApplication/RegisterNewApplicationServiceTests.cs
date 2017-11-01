using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Exceptions;
using Compass.Domain.Models;
using Compass.Domain.Services.KafkaProducer;
using Compass.Domain.Services.RegisterNewApplication;
using Xunit;
using FakeItEasy;

namespace Compass.Domain.Tests.Services.RegisterNewApplication
{
    
    public class RegisterNewApplicationServiceTests
    {
        private readonly RegisterNewApplicationService _sut;
        private readonly IDataStore _dataStore;
        private readonly IKafkaProducerService _kafkaProducerService;

        public RegisterNewApplicationServiceTests()
        {
            _dataStore = A.Fake<IDataStore>();
            _kafkaProducerService = A.Fake<IKafkaProducerService>();
            _sut = new RegisterNewApplicationService(_dataStore, _kafkaProducerService);
        }

        [Fact]
        public async Task RegisterNewApplication_ValidateUniqueAppName()
        {
            // Arrange
            var appName = "Rouge One";
            var registeredApplication = new RegisteredApplication
            {
                ApplicationName = appName,
                ApplicationToken = Guid.NewGuid(),
                DateCreated = DateTime.Now
            };

            A.CallTo(() => _dataStore.GetRegisteredApplicationAsync(appName))
             .Returns(Task.FromResult(registeredApplication));

            // Act
            async Task Action() => await _sut.RegisterNewApplicationAsync(appName);

            // Assert
            await Assert.ThrowsAsync<ApplicationAlreadyRegisteredException>(Action);
        }

        [Fact]
        public async Task RegisterNewApplication_Register()
        {
            // Arrange
            var appName = "Rouge One";

            A.CallTo(() => _dataStore.GetRegisteredApplicationAsync(appName))
             .Returns(Task.FromResult((RegisteredApplication)null));

            // Act
            await _sut.RegisterNewApplicationAsync(appName);

            // Assert
            A.CallTo(() => _dataStore.InsertAsync(A<RegisteredApplication>.That.Matches(arg => arg.ApplicationName == appName)))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task RegisterNewApplication_IdentifierMatchesAppToken()
        {
            // Arrange
            var appName = "Rouge One";

            A.CallTo(() => _dataStore.GetRegisteredApplicationAsync(appName))
             .Returns(Task.FromResult((RegisteredApplication)null));

            // Act
            await _sut.RegisterNewApplicationAsync(appName);

            // Assert
            A.CallTo(() => _dataStore.InsertAsync(A<RegisteredApplication>.That.Matches(arg => arg.ApplicationToken == arg.Identifier)))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task SendRegisteredApplicationToKafka()
        {
            // Arrange
            var appName = "Rouge One";

            A.CallTo(() => _dataStore.GetRegisteredApplicationAsync(appName))
                .Returns(Task.FromResult((RegisteredApplication)null));

            // Act
            await _sut.RegisterNewApplicationAsync(appName);

            // Assert
            A.CallTo(() => _kafkaProducerService.Produce(A<CompassEvent>.That.Matches(
                compassEvent => compassEvent.EventName == "ApplicationRegistered"
            ))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
