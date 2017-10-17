using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.QueueEvent
{
    public  interface IQueueEventService
    {
        Task QueueEventAsync(CompassEvent compassEvent);
        Task DeQueueEventAsync(CompassEvent compassEvent);
    }
}