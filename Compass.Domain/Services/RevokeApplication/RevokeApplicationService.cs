using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Exceptions;
using Compass.Domain.Models;

namespace Compass.Domain.Services.RevokeApplication
{
    public class RevokeApplicationService : IRevokeApplicationService
    {
        private readonly IDataStore _dataStore;

        public RevokeApplicationService(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<RegisteredApplication> RevokeApplicationAsync(Guid applicationToken)
        {
            var registeredApplication = await _dataStore.GetByDocumentIdAsync<RegisteredApplication>(applicationToken.ToString());
            
            if (registeredApplication == null)
            {
                throw new RegisteredApplicationNotFoundException(applicationToken);
            }
                
            registeredApplication.IsRevoked = true;
            await _dataStore.UpsertAsync(registeredApplication);
            return registeredApplication;
        }
    }
}
