using System.Threading.Tasks;
using Compass.Domain.Models;
using Compass.Domain.Services.Heartbeat;
using Microsoft.AspNetCore.Mvc;

namespace Compass.CoreServer.Controllers
{
    [Produces("application/json")]
    public class HeartbeatController : Controller
    {
        private readonly IHeartbeatService _heartbeatService;

        public HeartbeatController(
            IHeartbeatService heartbeatService
            )
        {
            _heartbeatService = heartbeatService;
        }

        [HttpPost("heartbeat")]
        public async Task Heartbeat([FromBody]ServiceSubscription serviceSubscription)
        {
            await _heartbeatService.Thump(serviceSubscription);
        }
    }
}