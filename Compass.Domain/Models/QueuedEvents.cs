using System.Collections.Generic;
using Compass.Domain.DataStore;

namespace Compass.Domain.Models
{
    public class QueuedEvents : Entity
    {
        public ICollection<CompassEvent> Events { get; set; } = new List<CompassEvent>();
        public override string DocType => nameof(QueuedEvents);
    }
}
