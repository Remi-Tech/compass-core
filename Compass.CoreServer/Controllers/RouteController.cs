using System.Threading.Tasks;
using Compass.Domain.Models;
using Compass.Domain.Services.RouteRequest;
using Microsoft.AspNetCore.Mvc;

namespace Compass.CoreServer.Controllers
{
    [Produces("application/json")]
    public class RouteController : Controller

    {
        private readonly IRouteRequestService _routeRequestService;

        public RouteController(
            IRouteRequestService routeRequestService
        )
        {
            _routeRequestService = routeRequestService;
        }

        [HttpPost("")]
        public async Task<CompassResult> RouteRequest([FromBody]CompassEvent compassEvent)
        {
            return await _routeRequestService.RouteRequest(compassEvent);
        }
    }
}