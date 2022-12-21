using NLog.Extensions.Logging;
using TextsExtractor.Options;
using TextsExtractor.Services;
using TextsExtractor.Services.Extractor;
using TextsExtractor.Services.Tika;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Trace);
        builder.AddNLog(hostContext.Configuration);
    });
    services.Configure<TikaServerOptions>(hostContext.Configuration.GetSection(TikaServerOptions.TikaServer));
    services.Configure<DirectoriesOptions>(hostContext.Configuration.GetSection(DirectoriesOptions.Directories));

    services.AddHttpClient<TikaHttpClient>();
    
    services.AddSingleton<IExtractor, Extractor>();
    services.AddHostedService<Worker>();
});

var app = builder.Build();
app.Run();