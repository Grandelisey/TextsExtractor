namespace TikaExtractor.Models;

public class TextFile
{
    public string Name { get; set; } = String.Empty;
    public string Text { get; set; } = String.Empty;
    public string ContentType { get; set; } = String.Empty;
    public string Language { get; set; } = String.Empty;
    public int ContentSize { get; set; }
}