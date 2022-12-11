using NLog.Extensions.Logging;
using TikaExtractor;
using TikaExtractor.Options;
using TikaExtractor.Services;

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
    services.AddSingleton<Extractor>();
    services.AddHostedService<Worker>();
});

var app = builder.Build();
app.Run();