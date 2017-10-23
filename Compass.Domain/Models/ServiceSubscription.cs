using System;
using System.Collections.Generic;
using Compass.Domain.DataStore;

namespace Compass.Domain.Models
{
    public class ServiceSubscription : Entity
    {
        public override Guid Identifier { get; set; }
        public Guid ApplicationToken { get; set; }
        public Uri ApplicationUri { get; set; }
        public ICollection<string> SubscribedEvents { get; set; } = new List<string>();
        public override string DocType => nameof(ServiceSubscription);
    }
}
