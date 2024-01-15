using DigitsWorker.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DigitsWorker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaprController : ControllerBase
    {
        private readonly ILogger<DaprController> _logger;
        private readonly PubSubSetting _appCfg;

        public DaprController(ILogger<DaprController> logger, IOptions<PubSubSetting> appCfg)
        {
            _logger = logger;
            _appCfg = appCfg.Value;
        }

        [HttpGet("[action]")]
        public Task<object> SubscribeCustom()
        {
            return Task.FromResult<object>(
           new List<object>{ new
           {
               PubSubName = _appCfg.PublishComp,
               _appCfg.Topic,
               Route = "/api/Work/Process",
               Metadata = new { RawPayload = false }

           }});
        }
    }
}
