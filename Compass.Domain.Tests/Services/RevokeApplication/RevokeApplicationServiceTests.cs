using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Exceptions;
using Compass.Domain.Models;
using Compass.Domain.Services.RevokeApplication;
using Xunit;
using FakeItEasy;

namespace Compass.Domain.Tests.Services.RevokeApplication
{
    
    public class RevokeApplicationServiceTests
    {
        private readonly IRevokeApplicationService _sut;
        private readonly IDataStore _dataStore;

        public RevokeApplicationServiceTests()
        {
            _dataStore = A.Fake<IDataStore>();
            _sut = new RevokeApplicationService(_dataStore);
        }

        [Fact]
        public async Task Revoke_ValidateAppToken()
        {
            // Arrange
            A.CallTo(() => _dataStore.GetByDocumentIdAsync<RegisteredApplication>(A<string>._))
             .Returns((RegisteredApplication)null);

            // Act
            async Task Action() => await _sut.RevokeApplicationAsync(Guid.NewGuid());

            // Assert
            await Assert.ThrowsAsync<RegisteredApplicationNotFoundException>(Action);
        }

        [Fact]
        public async Task Revoke_RevokeApplication()
        {
            // Arrange
            var appToken = Guid.NewGuid();
            var serviceApplication = new RegisteredApplication() {ApplicationToken = appToken, IsRevoked = false};
            A.CallTo(() => _dataStore.GetByDocumentIdAsync<RegisteredApplication>(A<string>._))
             .Returns(serviceApplication);

            // Act
            await _sut.RevokeApplicationAsync(appToken);

            // Assert
            A.CallTo(() => _dataStore.UpsertAsync(serviceApplication))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _dataStore.UpsertAsync(A<RegisteredApplication>.That.Matches(arg => arg.IsRevoked)))
             .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
