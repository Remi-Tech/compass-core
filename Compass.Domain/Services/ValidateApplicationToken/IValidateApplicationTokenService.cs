using System;
using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.ValidateApplicationToken
{
    public interface IValidateApplicationTokenService
    {
        Task<RegisteredApplication> ValidateApplicationTokenAsync(Guid applicationToken);
    }
}
