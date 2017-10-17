using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Compass.Domain.Models;
using Compass.Domain.Services.RouteRequest.SendToEndpoint;
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

        public async Task<dynamic[]> SendToEndpointAsync(IReadOnlyCollection<ServiceSubscription> subscriptions, dynamic payload)
        {
            var tasks = new List<Task<dynamic>>();

            subscriptions.ToList().ForEach(subscription => tasks.Add(SendAsync(subscription.ApplicationUri, payload)));
            return await Task.WhenAll(tasks);
        }

        private async Task<dynamic> SendAsync(Uri endpoint, dynamic payload)
        {
            const string header = "application/json";
            var client = GetHttpClient(header);
            var queryContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, header);

            var response = await _sendToEndpointPolicy.GetPolicy()
                .ExecuteAsync<HttpResponseMessage>(() => client.PostAsync(endpoint, queryContent));
            var result = await response.Content.ReadAsStringAsync();

            return result == null ? null : JsonConvert.DeserializeObject<dynamic>(result);
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
