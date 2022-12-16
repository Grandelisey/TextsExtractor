using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TikaExtractor.Models;

namespace TikaExtractor.Services;

public class Extractor
{
    private readonly ILogger<Extractor> _logger;
    private readonly TikaParser _tikaParser;
    private readonly TikaHttpClient _tikaHttpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public Extractor(
        ILogger<Extractor> logger, 
        TikaHttpClient tikaHttpClient)
    {
        _logger = logger;
        _tikaHttpClient = tikaHttpClient;
        _tikaParser = new TikaParser();
        
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
    }
    public async Task ExtractSingleJsonFile(FileInfo file, string destinationPath)
    {
        var jsonDocument = await _tikaHttpClient.GetMetaWithTextJsonAsync(file);
        var textFiles = _tikaParser.ParseTextFiles(jsonDocument);
        var validatedFiles = ExcludeEmptyFiles(textFiles);
        if (validatedFiles.Any())
        {
            var json = JsonSerializer.Serialize(validatedFiles, _jsonSerializerOptions);
            var outputFilePath = Path.Combine(destinationPath, FileService.AddNewExtension(file.Name, "json"));
            await FileService.WriteTextToFileAsync(json, outputFilePath);
            _logger.LogInformation("Extracted {ValidatedFilesCount} files", validatedFiles.Count());
        }
        else
        {
            _logger.LogInformation("Text files not extracted");
        }
    }
 
    private static IEnumerable<TextFile> ExcludeEmptyFiles(IEnumerable<TextFile> textFiles)
    {
        var validatedFiles = textFiles
            .Where(textFile => textFile.Text != String.Empty);
        
        return validatedFiles;
    }

}