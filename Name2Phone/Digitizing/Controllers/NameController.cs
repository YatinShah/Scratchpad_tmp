using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;

using Digitizing.Model;

using FluentValidation;

using Flurl;
using Flurl.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using NamingLib;

using Newtonsoft.Json;

namespace Digitizing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NameController : ControllerBase
{
    private readonly ILogger<NameController> _logger;
    private readonly AppCfgSetting _appCfg;
    private readonly ITokenizer _tokenizer;
    private readonly IValidator<string> _validator;
    private readonly DaprClient _daprClient;

    private const string _serviceActorName = "NameGenerator";

    public NameController(ILogger<NameController> logger, IOptions<AppCfgSetting> appCfg, ITokenizer tokenizer, IValidator<string> validator, DaprClient daprClient)
    {
        _logger = logger;
        _appCfg = appCfg.Value;
        _tokenizer = tokenizer;
        _validator = validator;
        _daprClient = daprClient;
    }
    [HttpGet("[action]/{name}")]
    public Task Digitize(string name)
    {
        var tokens = _tokenizer.Tokenize(name);
        var countryCode = tokens.First();
        var phoneName = tokens.Last();

        return _daprClient.PublishEventAsync(_appCfg.PubSub.PublishComp, _appCfg.PubSub.Topic, new PhoneDigits { CountryCode = countryCode, HandleSpecial = false, Name = phoneName });
    }


    [HttpPost("[action]")]
    public async Task<ActionResult> Result([FromBody] NewNr newPhoneNr)
    {
        var phoneNumber10Digits = newPhoneNr.Nr;

        var countryName = await _appCfg.IstdServiceBaseUrl
        .AppendPathSegment(_appCfg.IdentifyPath)
        .AppendPathSegment(newPhoneNr.CountryCode)
        .GetStringAsync();

        var formatted = await _appCfg.IstdServiceBaseUrl
        .AppendPathSegment(_appCfg.FormattedPath)
        .PostJsonAsync(new { CountryCode = newPhoneNr.CountryCode, Number = phoneNumber10Digits })
        .ReceiveString();

        var result = new List<string> { countryName, formatted };
        _logger.LogInformation($"received NewNr: {newPhoneNr.CountryCode}/{newPhoneNr.Nr}");
        _logger.LogInformation($"Formatted to : {countryName}:{formatted}");

        return Ok(new PhoneResult { Result = result, Status = "SUCCESS" }); //Yatin: (Dapr Gotcha!!), needs a return from a subscriber!!
    }


    [HttpGet("[action]/{name}")]
    public List<string> Elaborate(string name)
    {
        var result = new List<string>();
        foreach (var letter in name.ToUpper())
        {
            result.Add(Elaborate(letter));
        }
        return result;
    }


    [HttpPost("[action]/{number}")]
    public async Task<IEnumerable<string>> NumberToText(string number)
    {
        var deviceId = number.Substring(0, 3);
        var actorId = new ActorId(deviceId);
        var daprServicePort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT"); //Yatin: env independent way of getting dapr service port !!
        var nameGeneratorProxy = ActorProxy.Create<INameGen>(actorId, _serviceActorName, new ActorProxyOptions { RequestTimeout = TimeSpan.FromMinutes(1), HttpEndpoint = $"http://localhost:{daprServicePort}" });
        var phone2Names = await nameGeneratorProxy.GenerateNamesAsync(number.Substring(3));
        return phone2Names;

    }

    [HttpGet("TextToNumber/{name}")]
    public async Task<string> TextToNumberAsync(string name)
    {
        var tokens = _tokenizer.Tokenize(name);
        var countryCode = tokens.First();
        var phoneName = tokens.Last();

        var convertedNumber = await _daprClient.InvokeMethodAsync<PhoneDigits, NewNr>(HttpMethod.Post, _appCfg.WorkerServiceAppId, _appCfg.ProcessDigitsPath,
            new PhoneDigits
            {
                CountryCode = countryCode,
                HandleSpecial = false,
                Name = phoneName
            });
        return JsonConvert.SerializeObject(convertedNumber);
    }

    private string Elaborate(char letter)
    {
        switch (letter)
        {
            case 'A':
                return "Alpha";
            case 'B':
                return "Bravo";
            case 'C':
                return "Charlie";
            case 'D':
                return "Delta";
            case 'E':
                return "Echo";
            case 'F':
                return "Foxtrot";
            case 'G':
                return "Golf";
            case 'H':
                return "Hotel";
            case 'I':
                return "India";
            case 'J':
                return "Juliet";
            case 'K':
                return "Kilo";
            case 'L':
                return "Lima";
            case 'M':
                return "Mike";
            case 'N':
                return "November";
            case 'O':
                return "Oscar";
            case 'P':
                return "Papa";
            case 'Q':
                return "Quebec";
            case 'R':
                return "Romeo";
            case 'S':
                return "Sierra";
            case 'T':
                return "Tango";
            case 'U':
                return "Uniform";
            case 'V':
                return "Victor";
            case 'W':
                return "Whiskey";
            case 'X':
                return "X-ray";
            case 'Y':
                return "Yankee";
            case 'Z':
                return "Zulu";
            default:
                return "Unknown";
        }
    }


}
