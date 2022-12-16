using Microsoft.Extensions.Options;
using TikaExtractor.Options;

namespace TikaExtractor.Services;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Extractor _extractor;

    private string _rootDirectoryPath;
    private string _inputDirectoryPath = String.Empty;
    private string _outputDirectoryPath = String.Empty;
    private const string INPUT_DIRECTORY_NAME = "Input";
    private const string OUTPUT_DIRECTORY_NAME = "Output";

    public Worker(
        ILogger<Worker> logger, 
        IOptions<DirectoriesOptions> directoryOptions, 
        Extractor extractor)
    {
        _logger = logger;
        _extractor = extractor;
        _rootDirectoryPath = directoryOptions.Value.RootDirectory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TikaExtractor started");
        CreateDirectoriesStructure();
        while (!stoppingToken.IsCancellationRequested)
        {
            var inputDirectory = new DirectoryInfo(_inputDirectoryPath);
            await ProcessDirectory(inputDirectory, stoppingToken);
            await Task.Delay(5000, stoppingToken);
        }
    }
    
    private void CreateDirectoriesStructure()
    {
        var rootDirectory = Directory.CreateDirectory(_rootDirectoryPath);
        _rootDirectoryPath = rootDirectory.FullName;
        _inputDirectoryPath = rootDirectory.CreateSubdirectory(INPUT_DIRECTORY_NAME).FullName;
        _outputDirectoryPath = rootDirectory.CreateSubdirectory(OUTPUT_DIRECTORY_NAME).FullName;
        _logger.LogInformation("Directories structure created");
        
    }
    private async Task ProcessDirectory(DirectoryInfo directory, CancellationToken stoppingToken)
    {
        var files = directory.GetFiles();
        foreach (var file in files)
        {
            if (stoppingToken.IsCancellationRequested)
                return;
            try
            {   
                _logger.LogInformation("Start extracting file: {FileName}", file.Name);
                
                await _extractor.ExtractSingleJsonFile(file, _outputDirectoryPath).ConfigureAwait(false);
                
                _logger.LogInformation("Finish extracting file: {FileName}", file.Name);
                    
                file.Delete();
                _logger.LogInformation("Delited file: {FileName}", file.Name);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}