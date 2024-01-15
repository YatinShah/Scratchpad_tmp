using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;

namespace IstdApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class IstdController : ControllerBase
{
    private Dictionary<string, string> formatMap = new()
    {
        {"001",@"(?<area>\d{3,3})(?<first>\d{3,3})(?<nr>\d{4,4})"},
        {"044",@"(?<area>\d{3,3})(?<first>\d{3,3})(?<nr>\d{4,4})"},
        {"091",@"(?<area>\d{4,4})(?<nr>\d{6,6})"},
    };
    private Dictionary<string, string> map = new()
    {
        {"001","America/Canada"},
        {"044","United Kingdom"},
        {"091","India"}
    };
    private readonly ILogger<IstdController> _logger;

    public IstdController(ILogger<IstdController> logger)
    {
        _logger = logger;
    }

    [HttpGet("[action]/{countryCode}")]
    public Task<string> Identify(string countryCode)
    {
        _logger.LogInformation($"received {countryCode}");
        return Task.FromResult(map[countryCode]);
    }

    [HttpGet("Format/{countryCode}")]
    public Task<string> GetCountryFormat(string countryCode)
    {
        _logger.LogInformation($"{countryCode}");
        return Task.FromResult(formatMap[countryCode]);
    }

    [HttpPost("Formatted")]
    public Task<string> FormatByCountry([FromBody] PhoneNumber phoneNumber)
    {
        var format = formatMap[phoneNumber.CountryCode];
        var regEx = new Regex(format, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        var grps = regEx.Match(phoneNumber.Number).Groups;
        _logger.LogInformation($"parsed phone number {phoneNumber.CountryCode}/{phoneNumber.Number}");
        return Task.FromResult(string.Join("-", grps.Values.Skip(1).Select(x => x.Value)));
    }
}
