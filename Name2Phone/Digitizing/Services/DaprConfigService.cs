using Dapr.Client;

using Digitizing.Model;

using Microsoft.Extensions.Options;

namespace Digitizing.Services
{
    public class DaprConfigService
    {
        private readonly ILogger<DaprConfigService> _logger;
        private readonly AppCfgSetting _appCfg;
        private readonly DaprClientBuilder _daprClientBldr;

        public DaprConfigService(ILogger<DaprConfigService> logger, IOptions<AppCfgSetting> appCfg)
        {
            _logger = logger;
            _appCfg = appCfg.Value;
            _daprClientBldr = new DaprClientBuilder();

        }

        public async Task<string?> Get(string configKey)
        {
            using var daprClient = _daprClientBldr.Build();
            var configValues = await daprClient.GetConfiguration(_appCfg.ConfigStore, new List<string> { configKey });
            if (configValues == null || configValues.Items.Count <= 0) { return null; }
            return configValues.Items[configKey].Value;
        }
        public async Task<IEnumerable<KeyValuePair<string, string>>> Get(List<string> keys)
        {
            using var daprClient = _daprClientBldr.Build();
            var configValues = await daprClient.GetConfiguration(_appCfg.ConfigStore, keys);
            if (configValues == null || configValues.Items.Count <= 0) { return new List<KeyValuePair<string, string>>(); }
            return configValues.Items.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString()));
        }
        public async Task<T?> Get<T>(string configKey)
        {
            var value = await Get(configKey);
            if (value == null) return default;
            return (T?)Convert.ChangeType(value, typeof(T?));
        }
    }
}