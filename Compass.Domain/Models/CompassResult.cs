using System.Collections.Generic;

namespace Compass.Domain.Models
{
    public class CompassResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public List<dynamic> Response { get; set; } = new List<dynamic>();
    }
}
