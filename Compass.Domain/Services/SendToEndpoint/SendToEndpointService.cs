﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Compass.Domain.Models;
using Compass.Shared;
using Newtonsoft.Json;

namespace Compass.Domain.Services.SendToEndpoint
{
    public class SendToEndpointService : ISendToEndpointService
    {
        private readonly ICompassEnvironment _compassEnvironment;
        private readonly ISendToEndpointPolicy _sendToEndpointPolicy;

        public SendToEndpointService(
            ICompassEnvironment compassEnvironment,
            ISendToEndpointPolicy sendToEndpointPolicy
            )
        {
            _compassEnvironment = compassEnvironment;
            _sendToEndpointPolicy = sendToEndpointPolicy;
        }

        public async Task<IReadOnlyCollection<SendToEndpointResult>> SendToEndpointAsync(IReadOnlyCollection<ServiceSubscription> subscriptions, CompassEvent compassEvent)
        {
            var tasks = subscriptions.Select(async subscription =>
            {
                var sendToEndpointResult =
                    new SendToEndpointResult {ApplicationToken = subscription.ApplicationToken.ToString()};
                try
                {
                    sendToEndpointResult.Result = await SendAsync(subscription.ApplicationUri, compassEvent); 
                    sendToEndpointResult.Success = true;
                }
                catch (Exception)
                {
                    sendToEndpointResult.Success = false;
                }

                return sendToEndpointResult;
            }).ToList();

            return await Task.WhenAll(tasks);
        }

        private async Task<object> SendAsync(Uri endpoint, CompassEvent compassEvent)
        {
            const string header = "application/json";
            var client = GetHttpClient(header);
            var queryContent =
                new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        ApplicationToken = compassEvent.ApplicationToken,
                        ApplicationName = compassEvent.ApplicationName,
                        EventName = compassEvent.EventName,
                        Payload = compassEvent.Payload
                    }), Encoding.UTF8, header);

            var response = await _sendToEndpointPolicy.GetPolicy()
                .ExecuteAsync<HttpResponseMessage>(() => client.PostAsync(endpoint, queryContent));

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return result == null ? null : JsonConvert.DeserializeObject<object>(result);
        }

        private HttpClient GetHttpClient(string header)
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(_compassEnvironment.GetRequestTimeout())
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(header));
            return client;
        }
    }
}
