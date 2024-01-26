using System.Net.WebSockets;

using Dapr.Client;

using Digitizing.Model;

using Flurl;
using Flurl.Http;

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

        [HttpGet("[action]/{countryCode}")]
        public async Task<string> NonDaprEpResiliency(string countryCode)
        {
            var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
            var localDaprUrl = $"http://localhost:{daprPort}/v1.0/invoke";
            var countryName = await localDaprUrl
                .AppendPathSegment(_appCfg.IstdServiceBaseUrl)
                .AppendPathSegment("method")
                .AppendPathSegment(_appCfg.FormattedPath)
                .AppendPathSegment(countryCode)
                .GetStringAsync();
            return countryName;
        }

        [HttpGet("[action]/{namePhone}")]
        public async Task<NewNr> DaprEpResiliency(string namePhone)
        {
            var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
            var localDaprUrl = $"http://localhost:{daprPort}/v1.0/invoke";
            var convertedNumber = await localDaprUrl
                .AppendPathSegment(_appCfg.WorkerServiceAppId)
                .AppendPathSegment("method")
                .AppendPathSegment(_appCfg.ProcessDigitsPath)
                .PostJsonAsync(new PhoneDigits {CountryCode="001",HandleSpecial=false,Name= namePhone})
                .ReceiveJson<NewNr>();
            return convertedNumber;
        }

        [HttpGet("[action]/{durationInSec}")]
        public async Task<string> Delayed(int durationInSec)
        {
            var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
            var localDaprUrl = $"http://localhost:{daprPort}/v1.0/invoke";
            var response = await localDaprUrl
                .AppendPathSegment(_appCfg.WorkerServiceAppId)
                .AppendPathSegment("method")
                .AppendPathSegment(_appCfg.DelayedResponsePath)
                .AppendPathSegment(durationInSec)
                //.WithTimeout(durationInSec-15) //just to make sure we do timeout sooner at lower numbers tooooo
                .GetStringAsync();
            return response;
        }

        [HttpGet("[action]/{doThrow}")]
        public async Task<string> Fault(bool doThrow)
        {
            var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
            var localDaprUrl = $"http://localhost:{daprPort}/v1.0/invoke";
            var response = await localDaprUrl
                .AppendPathSegment(_appCfg.WorkerServiceAppId)
                .AppendPathSegment("method")
                .AppendPathSegment(_appCfg.FaultPath)
                .AppendPathSegment(doThrow)
                .GetStringAsync();
            return response;
        }

    }
}
