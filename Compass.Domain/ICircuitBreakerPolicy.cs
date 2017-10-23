using Polly;

namespace Compass.Domain
{
    public interface ICircuitBreakerPolicy
    {
        Policy GetCircuitBreakerPolicy();
    }
}
