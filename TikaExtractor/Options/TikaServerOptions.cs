namespace TikaExtractor.Options;

public class TikaServerOptions
{
    public const string TikaServer = "TikaServer";
    
    public string Host { get; set; } = String.Empty;
    public string Port { get; set; } = String.Empty;
}