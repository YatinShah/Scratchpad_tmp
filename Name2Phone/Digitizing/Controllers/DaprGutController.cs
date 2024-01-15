using Dapr.Client;

using Digitizing.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;


namespace Digitizing.Controllers
{
    [Route("api/dapr")]
    [ApiController]
    public class DaprGutController : ControllerBase
    {
        private readonly ILogger<DaprGutController> _logger;
        private readonly AppCfgSetting _appCfg;
        private readonly DaprClient _daprClient;

        public DaprGutController(ILogger<DaprGutController> logger, IOptions<AppCfgSetting> appCfg, DaprClient daprClient)
        {
            _logger = logger;
            _appCfg = appCfg.Value;
            _daprClient = daprClient;
        }

        [HttpGet("ConfigValues")]
        public async Task<IEnumerable<string?>> GetConfigValues()
        {
            var configValues = await _daprClient.GetConfiguration(_appCfg.ConfigStore, new List<string> { "key1", "key2" });
            return configValues?.Items.Select(x => JsonConvert.SerializeObject(x)).AsEnumerable() ?? new List<string>();
        }

        [HttpGet("StateValues/{key}")]
        public async Task<string> GetStateValues(string key)
        {
            var value = await _daprClient.GetStateAsync<string>(_appCfg.StateStore, key);
            return value;
        }

        [HttpPost("StateValues")]
        public Task SetStateValues([FromBody] StateValue state)
        {
            return _daprClient.SaveStateAsync<string>(_appCfg.StateStore, state.Key, state.Value);
        }

        [HttpGet("SecretValues/{secretname}")]
        public async Task<string> GetSecretValues(string secretname)
        {
            var value = await _daprClient.GetSecretAsync(_appCfg.SecretStore, secretname);
            return string.Join(Environment.NewLine, value.Select(x => $"{x.Key}-{x.Value}"));
        }

        [HttpGet("SidecarHealth")]
        public async Task<string> GetSidecarHealth()
        {
            var isHealthy = await _daprClient.CheckHealthAsync();
            return $"Both the sidecar and application are {(isHealthy ? "up and " : "not up or not ")}healthy";
        }

        [HttpGet("SidecarPartialHealth")]
        public async Task<string> GetSidecarPartialHealth()
        {
            var isHealthy = await _daprClient.CheckOutboundHealthAsync();
            return $"Dapr components {(isHealthy ? "up and " : "not up or not ")}healthy";
        }

        [HttpGet("Sidecar/{waitSecs}")]
        public async Task<string> CheckSidecarWorking(int waitSecs)
        {
            using (var tokenSource = new CancellationTokenSource(waitSecs * 1000))
                await _daprClient.WaitForSidecarAsync(tokenSource.Token);
            return $"Dapr side car is ready !!!";
        }

        [HttpDelete("Sidecar")]
        public async Task<string> RemoveSidecar()
        {
            await _daprClient.ShutdownSidecarAsync();
            return $"Dapr side car is shutdown!!!";
        }


        [HttpPost("/digicron")]
        [HttpPost("digicron")]
        public async Task CalledbyCron()
        {
            _logger.LogInformation($"Dapr Cron called me.. ;) @{DateTime.Now.ToShortTimeString()}");
        }
    }
}
