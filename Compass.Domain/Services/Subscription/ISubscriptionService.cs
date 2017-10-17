using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.Subscription
{
    public interface ISubscriptionService
    {
        Task<ServiceSubscription> SubscribeAsync(ServiceSubscription serviceSubcription);
    }
}