using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Exceptions;
using Compass.Domain.Models;
using Compass.Domain.Services.ValidateApplicationToken;
using Xunit;
using FakeItEasy;

namespace Compass.Domain.Tests.Services.ValidateApplicationToken
{
    
    public class ValidateApplicationTokenServiceTest
    {
        private readonly IValidateApplicationTokenService _sut;
        private readonly IDataStore _dataStore;

        public ValidateApplicationTokenServiceTest()
        {
            _dataStore = A.Fake<IDataStore>();
            _sut = new ValidateApplicationTokenService(_dataStore);
        }

        [Fact]
        public async Task ValidateAppToken_ApplicationDoesntExist()
        {
            // Arrange
            var applicationToken = Guid.NewGuid(); 
            A.CallTo(() => _dataStore.GetRegisteredApplicationAsync(applicationToken))
             .Returns(Task.FromResult((RegisteredApplication)null));

            // Act
            async Task Action() => await _sut.ValidateApplicationTokenAsync(applicationToken);

            // Assert
            await Assert.ThrowsAsync<RegisteredApplicationNotFoundException>(Action);
        }

        [Fact]
        public async Task ValidateAppToken_ApplicationRevoked()
        {
            // Arrange
            var applicationToken = Guid.NewGuid();
            A.CallTo(() => _dataStore.GetRegisteredApplicationAsync(applicationToken))
             .Returns(Task.FromResult(new RegisteredApplication() { IsRevoked = true }));

            // Act
            async Task Action() => await _sut.ValidateApplicationTokenAsync(applicationToken);

            //Assert
            await Assert.ThrowsAsync<RevokedApplicationException>(Action);
        }
    }
}
