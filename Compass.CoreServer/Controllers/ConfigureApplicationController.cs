using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Compass.Domain.DataStore;
using Compass.Domain.Models;
using Compass.Domain.Services.RegisterNewApplication;
using Compass.Domain.Services.RevokeApplication;
using Microsoft.AspNetCore.Mvc;

namespace Compass.CoreServer.Controllers
{
    [Produces("application/json")]
    [Route("config")]
    public class ConfigureApplicationController : Controller
    {
        private readonly IRegisterNewApplicationService _registerNewApplicationService;
        private readonly IRevokeApplicationService _revokeApplicationService;
        private readonly IDataStore _dataStore;

        public ConfigureApplicationController(
            IRegisterNewApplicationService registerNewApplicationService,
            IRevokeApplicationService revokeApplicationService,
            IDataStore dataStore
        )
        {
            _registerNewApplicationService = registerNewApplicationService;
            _revokeApplicationService = revokeApplicationService;
            _dataStore = dataStore;
        }

        [HttpPost("register/{applicationName}")]
        public async Task<RegisteredApplication> Register(string applicationName)
        {
            return await _registerNewApplicationService.RegisterNewApplicationAsync(applicationName);
        }

        [HttpPost("revoke/{applicationToken}")]
        public async Task<RegisteredApplication> RevokeToken(Guid applicationToken)
        {
            return await _revokeApplicationService.RevokeApplicationAsync(applicationToken);
        }

        [HttpGet("applications")]
        public async Task<IEnumerable<RegisteredApplication>> GetAllApplications()
        {
            return await _dataStore.GetAllRegisteredApplicationsAsync();
        }

    }
}