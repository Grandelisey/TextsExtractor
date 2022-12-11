using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TikaExtractor.Extensions;
using TikaExtractor.Models;
using TikaExtractor.Options;

namespace TikaExtractor.Services;

public class TikaService
{
    private readonly string _host;
    private readonly string _port;
    private string BaseUri => $"http://{_host}:{_port}";
    private readonly HttpClient _httpClient;


    private const string FILE_NAME_META_TAG = "resourceName";
    private const string CONTENT_META_TAG = "X-TIKA:content";
    private const string CONTENT_TYPE_META_TAG = "Content-Type";
    public TikaService(IOptions<TikaServerOptions> options)
    {
        _host = options.Value.Host;
        _port = options.Value.Port;

        _httpClient = new HttpClient();
    }

    public async Task<IAsyncEnumerable<TextFile>> ExtractTextFilesAsync(FileInfo file)
    {
        var fileBinaryContent = FileService.GetFileBinaryContent(file.FullName);
        var jsonDocument = await ExtractJsonAsync(fileBinaryContent);
        var extractedFiles = ParseTikaJsonResponse(jsonDocument);
        return extractedFiles;
    }
    private async Task<JsonDocument> ExtractJsonAsync(byte[] fileBinaryContent)
    {
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
    private async IAsyncEnumerable<TextFile> ParseTikaJsonResponse(JsonDocument jsonDocument)
    {
        var rootElement = jsonDocument.RootElement;
        if (rootElement.ValueKind == JsonValueKind.Array)
        {
            for (var i = 0; i < rootElement.GetArrayLength(); i++)
            {
                var fileJsonElement = rootElement[i];
                yield return await CreateExtractedTextFile(fileJsonElement);
            }
        }
        else
        {
            throw new JsonException("The root element of json is not an array");
        }
    }
    private async Task<TextFile> CreateExtractedTextFile(JsonElement jsonElement)
    {
        var text = ParseFileText(jsonElement);
        var language = await DetectLanguage(text);
        return new TextFile
        {
            Name = ParseFileName(jsonElement),
            Text = text,
            ContentType = ParseFileContentType(jsonElement),
            Language = language,
            ContentSize = text.Length
        };
    }
    private static string ParseFileName(JsonElement jsonFileElement)
    {
        jsonFileElement.TryGetProperty(FILE_NAME_META_TAG, out var nameElement);
        return nameElement.IsUndefined() ? String.Empty : nameElement.ToString().Trim();
    }
    private static string ParseFileText(JsonElement jsonFileElement)
    {
        jsonFileElement.TryGetProperty(CONTENT_META_TAG, out var textElement);
        return textElement.IsUndefined() ? String.Empty : textElement.ToString().Trim();
    }
    private static string ParseFileContentType(JsonElement jsonFileElement)
    {
        return jsonFileElement.GetProperty(CONTENT_TYPE_META_TAG).ToString();
    }
    private async Task<string> DetectLanguage(string text)
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