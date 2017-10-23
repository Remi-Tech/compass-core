using System;
using System.Collections.Generic;
using Compass.Domain.Exceptions;
using Compass.Domain.Models;
using Compass.Domain.Services.QueueEvent;
using Compass.Shared;
using Polly;

namespace Compass.Domain.Services.GetServiceSubscriptionsForEvent
{
    /// <summary>
    /// TODO: Use MediatR or contextual binding to simplify the interface.
    /// </summary>
    public class GetServiceSubscriptionsForEventPolicy : IGetServiceSubscriptionsForEventPolicy
    {
        private readonly ICompassEnvironment _compassEnvironment;
        private readonly IQueueEventService _queueEventService;

        public GetServiceSubscriptionsForEventPolicy(
            ICompassEnvironment compassEnvironment,
            IQueueEventService queueEventService
        )
        {
            _compassEnvironment = compassEnvironment;
            _queueEventService = queueEventService;
        }

        public Policy GetPolicy(CompassEvent compassEvent)
        {
            var fallback = Policy
                .Handle<NoValidSubscriptionsException>()
                .FallbackAsync(
                    (context, cancellationToken) => _queueEventService.QueueEventAsync(compassEvent),
                    (exception, context) => throw exception
                );

            var retry = Policy
                .Handle<NoValidSubscriptionsException>()
                .WaitAndRetryAsync(GetTimeSpans());

            return Policy
                .WrapAsync(fallback, retry);
        }

        private IEnumerable<TimeSpan> GetTimeSpans()
        {
            var timespans = new List<TimeSpan>();
            var retryAttempts = _compassEnvironment.GetRetryAttempts();

            for (var i = 0; i < retryAttempts; i++)
            {
                timespans.Add(TimeSpan.FromMilliseconds(_compassEnvironment.GetRetrySleepDuration()));
            }
            return timespans;
        }
    }
}
