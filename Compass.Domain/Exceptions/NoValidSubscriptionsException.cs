using System;
using Compass.Domain.Models;

namespace Compass.Domain.Exceptions
{
    public class NoValidSubscriptionsException : Exception
    {
        public CompassEvent CompassEvent { get; }

        public NoValidSubscriptionsException(CompassEvent compassEvent)
            : base($"There were no valid services subscribed to this event: {compassEvent.EventName}.")
        {
            CompassEvent = compassEvent;
        }
    }
}
