using AcApi.Controllers.Imp;
using AcApi.Infrastructure;
using AcApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AcApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceInitController : ControllerBase
    {
        private readonly ILogger<DeviceInitController> _logger;
        private readonly SmartCardRepository Configuration;
        private readonly IAccessControl _accessControl;
        private DeviceConnection _deviceConnection;


        public DeviceInitController(ILogger<DeviceInitController> logger, SmartCardRepository smartCard, IAccessControl accessControl, DeviceConnection deviceConnection)
        {
            _logger = logger;
            Configuration = smartCard;
            _accessControl = accessControl;
            _deviceConnection = deviceConnection;

        }

        [HttpGet]
        public object OnGet()
        {
            return Configuration.GetLastCard();
        }
    }
}
