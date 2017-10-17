using System;
using System.Net.Http;
using System.Threading.Tasks;
using Compass.Shared;
using Polly;

namespace Compass.Domain
{
    /// <summary>
    /// This class is registered as a singleton in the DI container.
    /// In order for the circuit breaker to work, we need a single
    /// instance across requests. This is because it holds state,
    /// the counter to determine the number of exceptions should live
    /// across requests.
    /// </summary>
    public class CircuitBreakerPolicy : ICircuitBreakerPolicy
    {
        private readonly Policy _policy;

        public CircuitBreakerPolicy(
            ICompassEnvironment compassEnvironment
            )
        {
            _policy = Policy
                .Handle<HttpRequestException>()
                // For http requests that take longer than
                // the request timeout limit.
                .Or<TaskCanceledException>()
                .CircuitBreakerAsync(compassEnvironment.GetExceptionsBeforeBreakingCircuit(),
                    TimeSpan.FromMilliseconds(compassEnvironment.GetCircuitBreakerDuration()));
        }

        public Policy GetCircuitBreakerPolicy()
        {
            return _policy;
        }
    }
}
