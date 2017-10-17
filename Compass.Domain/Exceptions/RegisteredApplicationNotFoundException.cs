using System;

namespace Compass.Domain.Exceptions
{
    public class RegisteredApplicationNotFoundException : Exception
    {
        public RegisteredApplicationNotFoundException(Guid applicationToken)
            : base($"Application with applicationToken: {applicationToken} does not exist.")
        {

        }
    }
}
