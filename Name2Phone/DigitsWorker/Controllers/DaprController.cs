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
        private const int MaxDelay = 300;
        public DaprController(ILogger<DaprController> logger, IOptions<PubSubSetting> appCfg)
        {
            _logger = logger;
            _appCfg = appCfg.Value;
        }


        /* Do not need this anymore...
        [HttpGet("[action]")]
        public Task<object> Subscriptions()
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
        */
        [HttpGet("[action]/{durationInSec}")]
        public async Task<string> DelayedResponse(int durationInSec)
        {
            var delay = TimeSpan.FromSeconds((durationInSec > MaxDelay || durationInSec <= 0) ? MaxDelay : durationInSec);
            await Task.Delay(delay);
            return $"Response was delayed by {delay.Seconds}";
        }

        [HttpGet("[action]/{doThrow}")]
        public string Fault(bool doThrow)
        {
            return doThrow ? throw new ArgumentException("Exception as requested", nameof(doThrow)) : "No fault creation was requested";
        }
    }
}
