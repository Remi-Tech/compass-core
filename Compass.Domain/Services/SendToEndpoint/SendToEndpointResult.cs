using System;
using System.Collections.Generic;
using System.Text;

namespace Compass.Domain.Services.SendToEndpoint
{
    public class SendToEndpointResult
    {
        public bool Success { get; set; } = true;
        public string ApplicationToken { get; set; }
        public object Result { get; set; }
    }
}
