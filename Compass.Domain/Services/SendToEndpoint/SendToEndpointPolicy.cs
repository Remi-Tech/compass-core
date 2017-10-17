using System.Net.Http;
using System.Threading.Tasks;
using Compass.Shared;
using Polly;

namespace Compass.Domain.Services.SendToEndpoint
{
    public class SendToEndpointPolicy : ISendToEndpointPolicy
    {
        private readonly ICompassEnvironment _compassEnvironment;
        private readonly ICircuitBreakerPolicy _circuitBreakerPolicy;

        public SendToEndpointPolicy(
            ICompassEnvironment compassEnvironment,
            ICircuitBreakerPolicy circuitBreakerPolicy
            )
        {
            _compassEnvironment = compassEnvironment;
            _circuitBreakerPolicy = circuitBreakerPolicy;
        }

        public Policy GetPolicy()
        {
            var retry = Policy
                .Handle<HttpRequestException>()
                // For http requests that take longer than
                // the request timeout limit.
                .Or<TaskCanceledException>()
                .RetryAsync(_compassEnvironment.GetRetryAttempts());

            return Policy.WrapAsync(_circuitBreakerPolicy.GetCircuitBreakerPolicy(), retry);
        }
    }
}
