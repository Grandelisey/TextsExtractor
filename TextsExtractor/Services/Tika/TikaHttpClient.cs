using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TextsExtractor.Options;

namespace TextsExtractor.Services.Tika;

public class TikaHttpClient
{
    private readonly HttpClient _client;

    public TikaHttpClient(HttpClient client, IOptions<TikaServerOptions> options)
    {
        var host = options.Value.Host;
        var port = options.Value.Port;
        var timeoutSeconds = options.Value.TimeoutSeconds;
        
        _client = client;
        _client.BaseAddress = new Uri($"http://{host}:{port}");
        _client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
    }
    
    public async Task<JsonDocument> GetTextsWithMetadataAsync(FileInfo file)
    {
        var fileBinaryContent = FileService.GetFileBinaryContent(file.FullName);
        const string uri = "rmeta/text";
        HttpContent requestContent = new ByteArrayContent(fileBinaryContent);
        var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = requestContent,
            Headers =
            {
                {"Accept", @"application/json"}
            }
        };
        var response = await _client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        return jsonDocument;
    }
    public async Task<string> GetLanguageAsync(string text)
    {
        const string uri = "language/stream";
        var binaryContent = Encoding.UTF8.GetBytes(text);
        var requestContent = new ByteArrayContent(binaryContent);
        var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = requestContent
        };
        var response = await _client.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    } 
}