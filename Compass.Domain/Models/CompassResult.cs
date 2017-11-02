using System.Collections.Generic;

namespace Compass.Domain.Models
{
    public class CompassResult
    {
        public ICollection<string> Messages { get; set; }
        public CompassResponseStatus Status { get; set; }
        public object Payload { get; set; } = new List<object>();
    }
}
