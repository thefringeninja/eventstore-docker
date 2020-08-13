using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SampleApp {
    [Route("api")]
    public class TestController : ControllerBase {
        readonly ILogger<TestController> _logger;
        readonly IEnumerable<IPAddress>  _addresses;

        public TestController(ILogger<TestController> logger) {
            _logger = logger;

            _addresses = NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(x => x.GetIPProperties().UnicastAddresses)
                .Select(c => c.Address);
        }

        [Route("test")]
        public string Get(string xxx) {
            _logger.LogInformation(Request.Host.Value);
            var address = Request.HttpContext.Connection.LocalIpAddress;
            var matches   = _addresses.Where(x => x.Equals(address));

            foreach (var match in matches) {
                _logger.LogInformation("Matched {@address}", match);
            }
            return "";
        }
    }
}
