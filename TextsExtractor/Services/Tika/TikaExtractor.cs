using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TextsExtractor.Models;
using TextsExtractor.Services.Extractor;

namespace TextsExtractor.Services.Tika;

public class Extractor : IExtractor
{
    private readonly ILogger<Extractor> _logger;
    private readonly TikaHttpClient _tikaHttpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public Extractor(
        ILogger<Extractor> logger, 
        TikaHttpClient tikaHttpClient)
    {
        _logger = logger;
        _tikaHttpClient = tikaHttpClient;
        
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
    }
    public async Task ExtractJsonAsync(FileInfo file, string destinationPath)
    {
        var jsonDocument = await _tikaHttpClient.GetTextsWithMetadataAsync(file);
        var textFiles = TikaParser.ParseTextFiles(jsonDocument);
        await DetectTextsLanguagesAsync(textFiles);
        if (textFiles.Any())
        {
            var json = JsonSerializer.Serialize(textFiles, _jsonSerializerOptions);
            var outputFilePath = Path.Combine(destinationPath, FileService.AddNewExtension(file.Name, "json"));
            await FileService.WriteTextToFileAsync(json, outputFilePath);
            _logger.LogInformation("Extracted {ValidatedFilesCount} files", textFiles.Count());
        }
        else
        {
            _logger.LogInformation("Text files not extracted");
        }
    }

    private async Task DetectTextsLanguagesAsync(List<TextFile> textFiles)
    {
        _logger.LogInformation("Start detecting languages");
        foreach (var textFile in textFiles)
        {
            textFile.Language = await _tikaHttpClient.GetLanguageAsync(textFile.Text);
        }
        _logger.LogInformation("Finish detecting languages");
    }
    
}