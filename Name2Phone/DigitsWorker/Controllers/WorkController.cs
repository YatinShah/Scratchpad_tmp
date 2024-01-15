using System.Text.Json.Serialization;

using Dapr.Client;

using DigitsWorker.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace DigitsWorker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkController : ControllerBase
    {
        private ILogger<Program> _logger;
        private PubSubSetting _settings;
        private IDigitService _srv;
        private readonly DaprClient _daprClient;

        public WorkController(ILogger<Program> logger, IOptions<PubSubSetting> settings, IDigitService srv, DaprClient daprClient)
        {
            _logger = logger;
            _settings = settings.Value;
            _srv = srv;
            _daprClient = daprClient;
        }

        [HttpPost("Process")]
        [HttpPost("/process")]  //2nd shorter route
        public async Task<ActionResult> ProcessAsync([FromBody] PhoneDigits input)
        {
            _logger.LogInformation($"received {input.Name} to convert");
            var newPhoneNr = await _srv.ProcessDigitsAsync(input);

            await _daprClient.PublishEventAsync(_settings.PublishComp, _settings.Topic, new NewNr { Nr = newPhoneNr, CountryCode = input.CountryCode });
            _logger.LogInformation($"sent {newPhoneNr}");
            return Ok(new { status = "SUCCESS" }); // we need to return a response of this type for a success !! (Dapr Gotcha!!)

        }

        [HttpPost("ProcessDigits")]
        public async Task<ActionResult> ProcessDigits([FromBody] PhoneDigits input)
        {
            _logger.LogInformation($"received {input.Name} to convert");
            var newPhoneNr = await _srv.ProcessDigitsAsync(input);

            _logger.LogInformation($"generated {newPhoneNr}");
            return Ok(new NewNr { CountryCode = input.CountryCode, Nr = newPhoneNr }); // we need to return a response of this type for a success !! (Dapr Gotcha!!)

        }
    }
}