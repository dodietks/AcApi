using Microsoft.Extensions.Configuration;
using AcApi.Models;
using Microsoft.Extensions.Logging;

namespace AcApi.Controllers
{
    public class Device
    {
        private readonly ILogger<Device> _logger;
        private readonly IConfiguration Configuration;

        public int m_UserID = -1;

        public Device(ILogger<Device> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }
        public int Init() {
            var login = new LoginOptions();
            Configuration.GetSection(LoginOptions.ACS).Bind(login);
            
            
            return m_UserID;
        }
        
    }
}
