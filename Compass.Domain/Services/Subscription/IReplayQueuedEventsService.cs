using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.Subscription
{
    public interface IReplayQueuedEventsService
    {
        Task ReplayQueuedEvents(ServiceSubscription serviceSubscription);
    }
}
