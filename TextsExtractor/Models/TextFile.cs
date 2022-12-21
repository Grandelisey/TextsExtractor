namespace TextsExtractor.Models;

public class TextFile
{
    public string Text { get; init; } = String.Empty;
    public string ContentType { get; init; } = String.Empty;
    public string Language { get; set; } = String.Empty;
    public int ContentSize { get; set; }
}