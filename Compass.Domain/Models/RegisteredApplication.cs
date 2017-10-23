using System;
using System.Collections.Generic;
using Compass.Domain.DataStore;

namespace Compass.Domain.Models
{
    public class RegisteredApplication : Entity
    {
        public string ApplicationName { get; set; }
        public Guid ApplicationToken { get; set; }
        public DateTime? LastSeen { get; set; }
        public IReadOnlyCollection<string> LastEventsSubscribed { get; set; } = new List<string>();
        public bool IsRevoked { get; set; }
        public DateTime DateCreated { get; set; }
        public override string DocType => nameof(RegisteredApplication);
    }
}
