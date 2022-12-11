using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.Extensions.Options;
using TikaExtractor.Models;
using TikaExtractor.Options;

namespace TikaExtractor.Services;

public class Extractor
{
    private readonly ILogger<Extractor> _logger;
    private readonly TikaService _tikaService;

    public Extractor(IOptions<TikaServerOptions> tikaServerOptions, ILogger<Extractor> logger)
    {
        _logger = logger;
        _tikaService = new TikaService(tikaServerOptions);
    }
    public async Task ExtractSingleJsonFile(FileInfo file, string destinationPath)
    {
        var textFiles = await _tikaService.ExtractTextFilesAsync(file);
        var validatedFiles = ValidateExtractedFiles(file, textFiles.ToBlockingEnumerable().ToList());
        if (validatedFiles.Any())
        {
            var json = CreateOutputJsonString(validatedFiles);
            var outputFilePath = Path.Combine(destinationPath, FileService.AddNewExtension(file.Name, "json"));
            await FileService.WriteTextToFileAsync(json, outputFilePath);
            _logger.LogInformation("Extracted {ValidatedFilesCount} files", validatedFiles.Count);
        }
        else
        {
            _logger.LogInformation("Text files not extracted");
        }
    }
    public async Task ExtractTextFiles(FileInfo file, string destinationPath)
    {
        var textFiles = await _tikaService.ExtractTextFilesAsync(file);
        var validatedFiles = ValidateExtractedFiles(file, textFiles.ToBlockingEnumerable().ToList());
        if (validatedFiles.Any())
        {
            var outputDirectory = FileService.CreateOutputDirectory(file, destinationPath);
            foreach (var textFile in validatedFiles)
            {
                var textFilePath = Path.Combine(outputDirectory.FullName, textFile.Name);
                await FileService.WriteTextToFileAsync(textFile.Text, textFilePath);
            }
            _logger.LogInformation("Extracted {ValidatedFilesCount} files", validatedFiles.Count);
        }
        else
        {
            _logger.LogInformation("Text files not extracted");
        }
    }
    
    private static List<TextFile> ValidateExtractedFiles(FileInfo originFile, IEnumerable<TextFile> textFiles)
    {
        var validatedFiles = textFiles
            .Where(textFile => textFile.Text != String.Empty)
            .ToList();

        var singleFileExtracted = validatedFiles.Count() == 1;
        if (singleFileExtracted)
        {
            validatedFiles[0].Name = originFile.Name;
        }

        var unnamedFilesCounter = 0;
        foreach (var file in validatedFiles.Where(file => String.IsNullOrEmpty(file.Name)))
        {
            if (unnamedFilesCounter == 0)
            {
                file.Name = originFile.Name;
                unnamedFilesCounter++;
            }
            else
                file.Name = $"{originFile.Name}({unnamedFilesCounter++})";
        }

        return validatedFiles;
    }
    private static string CreateOutputJsonString(IEnumerable<TextFile> files)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        };
        return JsonSerializer.Serialize(files, options);
    }
}