using System.Threading.Tasks;
using Compass.Domain.Models;
using Compass.Domain.Services.Subscription;
using Microsoft.AspNetCore.Mvc;

namespace Compass.CoreServer.Controllers
{
    [Produces("application/json")]
    public class SubscribeController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscribeController(
            ISubscriptionService subscriptionService
        )
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("subscribe")]
        public async Task<ServiceSubscription> Subscribe([FromBody]ServiceSubscription serviceSubscription)
        {
            return new ServiceSubscription();
            return await _subscriptionService.SubscribeAsync(serviceSubscription);
        }

    }
}