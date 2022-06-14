using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace dotnet_restapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppConfigController : ControllerBase
    {
        private static IConfiguration _configuration;
        private readonly ILogger<AppConfigValues> _logger;

        public AppConfigController(ILogger<AppConfigValues> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<AppConfigValues> Get()
        {
            var messages = new List<AppConfigValues>();
            messages.Add( new AppConfigValues{
                Date = DateTime.Now,
                DirectReference = _configuration["config::basic::test001"],
                KeyVaultReference = _configuration["config::kv::test001"]
            });
            return messages.ToArray();
        }
    }
}
