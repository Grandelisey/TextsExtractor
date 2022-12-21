namespace TextsExtractor.Options;

public class TikaServerOptions
{
    public const string TikaServer = "TikaServer";
    
    public string Host { get; set; } = String.Empty;
    public string Port { get; set; } = String.Empty;
    
    public int TimeoutSeconds { get; set; } = 300;
}