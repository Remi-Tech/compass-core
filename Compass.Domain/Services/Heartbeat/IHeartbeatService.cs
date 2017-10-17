using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.Heartbeat
{
    public interface IHeartbeatService
    {
        Task Thump(ServiceSubscription serviceSubscription);
    }
}
