using System.Collections.Generic;

namespace Compass.Domain.Models
{
    public class CompassResult
    {
        public ICollection<string> Message { get; set; }
        public CompassResponseStatus Success { get; set; }
        public object Response { get; set; } = new List<object>();
    }
}
