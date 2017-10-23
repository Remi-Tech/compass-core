using System;
using System.Threading.Tasks;
using Compass.Domain.Models;

namespace Compass.Domain.Services.RevokeApplication
{
    public interface IRevokeApplicationService
    {
        Task<RegisteredApplication> RevokeApplicationAsync(Guid applicationToken);
    }
}
