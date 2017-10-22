using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.KafkaStream;
using Compass.Domain.Services.ValidateApplicationToken;
using Compass.Domain.Services.GetServiceSubscriptionsForEvent;
using Compass.Domain.Services.SendToEndpoint;

namespace Compass.Domain.Services.RouteRequest
{
    public class RouteRequestService : IRouteRequestService
    {
        private readonly ISendToEndpointService _sendToEndpointService;
        private readonly IValidateApplicationTokenService _validateApplicationTokenService;
        private readonly IKafkaStreamService _kafkaStreamService;
        private readonly IGetServiceSubscriptionsForEventService _getServiceSubscriptionsForEventService;
        private readonly IDataStore _dataStore;

        public RouteRequestService(
            ISendToEndpointService sendToEndpointService,
            IValidateApplicationTokenService validateApplicationTokenService,
            IKafkaStreamService kafkaStreamService,
            IGetServiceSubscriptionsForEventService getServiceSubscriptionsForEventService,
            IDataStore dataStore
            )
        {
            _sendToEndpointService = sendToEndpointService;
            _validateApplicationTokenService = validateApplicationTokenService;
            _kafkaStreamService = kafkaStreamService;
            _getServiceSubscriptionsForEventService = getServiceSubscriptionsForEventService;
            _dataStore = dataStore;
        }

        public async Task<CompassResult> RouteRequest(CompassEvent compassEvent)
        {
            await _validateApplicationTokenService.ValidateApplicationTokenAsync(compassEvent.ApplicationToken);
            var subscriptions =
                await _getServiceSubscriptionsForEventService.GetServiceSubscriptionsAsync(compassEvent);

            _kafkaStreamService.StreamToKafka(compassEvent);

            return await SendToSubscriptionsAsync(compassEvent, subscriptions);
        }

        private async Task<CompassResult> SendToSubscriptionsAsync(CompassEvent compassEvent, IReadOnlyCollection<ServiceSubscription> subscriptions)
        {
            var registeredApplicationTask =
                _dataStore.GetByDocumentsIdAsync<RegisteredApplication>(
                    subscriptions.Select(subscription => subscription.ApplicationToken.ToString()));
            var sendToEndpointTask = _sendToEndpointService.SendToEndpointAsync(subscriptions, compassEvent.EventName, compassEvent.Payload);
            await Task.WhenAll(registeredApplicationTask, sendToEndpointTask);
            var registeredApplications = await registeredApplicationTask;
            var responses = await sendToEndpointTask;
            var compassResult = new CompassResult { Success = CompassResponseStatus.Success };

            if (responses != null)
            {
                compassResult.Success = GetCompassResponseStatus(responses);
                compassResult.Response = BuildResponse(registeredApplications, responses);
            }

            return compassResult;
        }

        private CompassResponseStatus GetCompassResponseStatus(IReadOnlyCollection<SendToEndpointResult> responses)
        {
            if (responses.All(response => !response.Success)) return CompassResponseStatus.Failure;
            if (responses.Any(response => !response.Success)) return CompassResponseStatus.PartialFailure;
            return CompassResponseStatus.Success;
        }

        private dynamic BuildResponse(IReadOnlyCollection<RegisteredApplication> applications,
            IReadOnlyCollection<SendToEndpointResult> endpointResults)
        {
            var response = new ExpandoObject();

            endpointResults.ToList().ForEach(result =>
            {
                var application = applications
                    .Single(app => app.ApplicationToken.ToString().Equals(result.ApplicationToken)).ApplicationName;
                response.TryAdd(ConvertToCamelCase(application), result.Result ?? new { });
            });

            return response;
        }

        private static string ConvertToCamelCase(string name)
        {
            var result = CultureInfo
                .InvariantCulture
                .TextInfo
                .ToTitleCase(name.ToLowerInvariant().Trim())
                .Replace("_", "")
                .Replace("-", "")
                .Replace(".", "");
            return char.ToLowerInvariant(result[0]) + result.Substring(1);
        }
    }
}