using System;
using System.Collections.Generic;
using System.Text;

namespace Compass.Domain.Models
{
    public enum CompassResponseStatus
    {
        Success = 0,
        PartialFailure = 1,
        Failure = 2
    }
}
