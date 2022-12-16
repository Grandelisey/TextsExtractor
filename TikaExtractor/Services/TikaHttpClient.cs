using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TikaExtractor.Options;

namespace TikaExtractor.Services;

public class TikaHttpClient
{
    private readonly string _host;
    private readonly string _port;
    private string BaseUri => $"http://{_host}:{_port}";
    private readonly HttpClient _httpClient;

    public TikaHttpClient(IOptions<TikaServerOptions> options)
    {
        _host = options.Value.Host;
        _port = options.Value.Port;
        
        var timeoutSeconds = options.Value.TimeoutSeconds;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };
    }
    
    public async Task<JsonDocument> GetMetaWithTextJsonAsync(FileInfo file)
    {
        var fileBinaryContent = FileService.GetFileBinaryContent(file.FullName);
        var uri = $"{BaseUri}/rmeta/text";
        HttpContent requestContent = new ByteArrayContent(fileBinaryContent);
        var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = requestContent,
            Headers =
            {
                Accept = {MediaTypeWithQualityHeaderValue.Parse(@"application/json")}
            }
        };
        var response = await _httpClient.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseString);
        return jsonDocument;
    }
    public async Task<string> GetLanguageAsync(string text)
    {
        var uri = $"{BaseUri}/language/stream";
        var binaryContent = Encoding.UTF8.GetBytes(text);
        var requestContent = new ByteArrayContent(binaryContent);
        var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = requestContent
        };
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    } 
}