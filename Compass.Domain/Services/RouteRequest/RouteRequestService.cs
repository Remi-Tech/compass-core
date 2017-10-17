using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.Models;
using Compass.Domain.Services.KafkaStream;
using Compass.Domain.Services.ValidateApplicationToken;
using Compass.Domain.Services.GetServiceSubscriptionsForEvent;
using Compass.Domain.Services.RouteRequest.SendToEndpoint;

namespace Compass.Domain.Services.RouteRequest
{
    public class RouteRequestService : IRouteRequestService
    {
        private readonly ISendToEndpointService _sendToEndpointService;
        private readonly IValidateApplicationTokenService _validateApplicationTokenService;
        private readonly IKafkaStreamService _kafkaStreamService;
        private readonly IGetServiceSubscriptionsForEventService _getServiceSubscriptionsForEventService;

        public RouteRequestService(
            ISendToEndpointService sendToEndpointService,
            IValidateApplicationTokenService validateApplicationTokenService,
            IKafkaStreamService kafkaStreamService,
            IGetServiceSubscriptionsForEventService getServiceSubscriptionsForEventService
            )
        {
            _sendToEndpointService = sendToEndpointService;
            _validateApplicationTokenService = validateApplicationTokenService;
            _kafkaStreamService = kafkaStreamService;
            _getServiceSubscriptionsForEventService = getServiceSubscriptionsForEventService;
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
            var response = await _sendToEndpointService.SendToEndpointAsync(subscriptions, compassEvent.Payload);
            var compassResult = new CompassResult {Success = true};

            if(response != null)
            {
                compassResult.Response.Add(response);
            }

            return compassResult;
        }
    }
}