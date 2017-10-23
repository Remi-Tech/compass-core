using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Exceptions;
using Compass.Domain.Models;

namespace Compass.Domain.Services.ValidateApplicationToken
{
    public class ValidateApplicationTokenService : IValidateApplicationTokenService
    {
        private readonly IDataStore _dataStore;

        public ValidateApplicationTokenService(
            IDataStore dataStore
            )
        {
            _dataStore = dataStore;
        }
        public async Task<RegisteredApplication> ValidateApplicationTokenAsync(Guid applicationToken)
        {
            var registeredApplication = await _dataStore.GetRegisteredApplicationAsync(applicationToken);

            Validate(registeredApplication, applicationToken);

            return registeredApplication;
        }

        // ReSharper disable once UnusedParameter.Local
        private void Validate(RegisteredApplication registeredApplication, Guid applicationToken)
        {
            if (registeredApplication == null)
            {
                throw new RegisteredApplicationNotFoundException(applicationToken);
            }

            if (registeredApplication.IsRevoked)
            {
                throw new RevokedApplicationException();
            }
        }

    }
}
