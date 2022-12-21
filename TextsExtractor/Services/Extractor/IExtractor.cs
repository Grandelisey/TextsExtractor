namespace TextsExtractor.Services.Extractor;

public interface IExtractor
{
    public Task ExtractJsonAsync(FileInfo file, string destinationPath);
}