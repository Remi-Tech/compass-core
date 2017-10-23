using System;

namespace Compass.Domain.Exceptions
{
    public class RevokedApplicationException : Exception
    {
        public RevokedApplicationException() 
            : base("This application has been revoked")
        {

        }

    }
}
