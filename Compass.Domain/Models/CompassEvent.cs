using System;
using Compass.Domain.DataStore;

namespace Compass.Domain.Models
{
    public class CompassEvent : Entity
    {
        public string UserId { get; set; }
        public string EventName { get; set; }
        /// <summary>
        /// The token of the application from which this event originated.
        /// </summary>
        public Guid ApplicationToken { get; set; }
        /// <summary>
        /// The name of the application from which this event originated.
        /// </summary>
        public string ApplicationName { get; set; }
        public object Payload { get; set; }
        internal DateTime DateCreated { get; set; }
        public override string DocType => nameof(CompassEvent);
    }
}
