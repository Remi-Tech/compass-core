using System;

namespace Compass.Domain.Exceptions
{
    public class ApplicationAlreadyRegisteredException : Exception
    {
        public ApplicationAlreadyRegisteredException(string applicationName)
            : base($"Application {applicationName} is already registered")
        {
        }
    }
}
