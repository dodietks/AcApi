using AcApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AcApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceInitController : ControllerBase
    {
        private readonly ILogger<DeviceInitController> _logger;
        private readonly SmartCardRepository Configuration;
        private DeviceConnection _deviceConnection;


        public DeviceInitController(ILogger<DeviceInitController> logger, SmartCardRepository smartCard, DeviceConnection deviceConnection)
        {
            _logger = logger;
            Configuration = smartCard;
            _deviceConnection = deviceConnection;
        }

        [HttpGet]
        public object OnGet()
        {
            return Configuration.GetLastCard();
        }
    }
}
