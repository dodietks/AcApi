using AcApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceInit : ControllerBase
    {
        private readonly ILogger<DeviceInit> _logger;
        private readonly IConfiguration Configuration;

        public DeviceInit(ILogger<DeviceInit> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        [HttpGet]
        public ContentResult OnGet()
        {
            var login = new LoginOptions();
            Configuration.GetSection(LoginOptions.ACS).Bind(login);

            return Content($"Nome: {login.Name}  \n" +
                           $"Senha: {login.Password} \n" +
                           $"Ip: {login.Ip} \n" +
                           $"Porta: {login.Port}");
        }
    }
}
