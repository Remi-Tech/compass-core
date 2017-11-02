using System;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Exceptions;
using Compass.Domain.Models;
using Compass.Domain.Services.KafkaProducer;

namespace Compass.Domain.Services.RegisterNewApplication
{
    public class RegisterNewApplicationService : IRegisterNewApplicationService
    {
        private readonly IDataStore _dataStore;
        private readonly IKafkaProducerService _kafkaProducerService;

        public RegisterNewApplicationService(
            IDataStore dataStore,
            IKafkaProducerService kafkaProducerService
            )
        {
            _dataStore = dataStore;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<RegisteredApplication> RegisterNewApplicationAsync(string applicationName)
        {
            await ValidateApplicationDoesntExist(applicationName);

            var registeredApplication = GetRegisteredApplicationModel(applicationName);
            await _dataStore.InsertAsync(registeredApplication);

            SendRegisteredApplicationToKafka(registeredApplication);

            return registeredApplication;
        }

        private async Task ValidateApplicationDoesntExist(string applicationName)
        {
            var registeredApplication = await _dataStore.GetRegisteredApplicationAsync(applicationName);

            if (registeredApplication != null)
            {
                throw new ApplicationAlreadyRegisteredException(applicationName);
            }
        }

        private static RegisteredApplication GetRegisteredApplicationModel(string applicationName)
        {
            var applicationToken = Guid.NewGuid();
            return new RegisteredApplication
            {
                Identifier = applicationToken,
                ApplicationToken = applicationToken,
                ApplicationName = applicationName,
                DateCreated = DateTime.UtcNow
            };
        }

        private void SendRegisteredApplicationToKafka(RegisteredApplication registeredApplication)
        {
            _kafkaProducerService.Produce(new CompassEvent
            {
                ApplicationToken = registeredApplication.ApplicationToken,
                DateCreated = DateTime.UtcNow,
                EventName = "ApplicationRegistered",
                Payload = new {registeredApplication.ApplicationName}
            });
        }
    }
}
