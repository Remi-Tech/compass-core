using Polly;

namespace Compass.Domain.Services.SendToEndpoint
{
    public interface ISendToEndpointPolicy
    {
        Policy GetPolicy();
    }
}
