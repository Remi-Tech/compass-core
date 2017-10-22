using System;
using Compass.Domain.DataStore;

namespace Compass.Domain.Models
{
    public class CompassEvent : Entity
    {
        public string UserId { get; set; }
        public string EventName { get; set; }
        public Guid ApplicationToken { get; set; }
        public object Payload { get; set; }
        internal DateTime DateCreated { get; set; }
        public override string DocType => nameof(CompassEvent);
    }
}
