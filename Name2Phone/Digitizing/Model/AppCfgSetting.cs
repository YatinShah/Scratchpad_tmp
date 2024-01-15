namespace Digitizing.Model;

public class AppCfgSetting
{
    public required string IstdServiceBaseUrl { get; set; }
    public required string IdentifyPath { get; set; }
    public required string FormatPath { get; set; }
    public required string FormattedPath { get; set; }
    public required string WorkerServiceAppId { get; set; }
    public required string ProcessDigitsPath { get; set; }

    public required PubSubSetting PubSub { get; set; } 
    public required string StateStore { get; set; }
    public required string ConfigStore { get; set; }
    public required string SecretStore { get; set; }
    
}

public class PubSubSetting
{
    public string PublishComp { get; set; }
    public string Topic { get; set; }
}

