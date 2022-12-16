using System.Text.Json;
using TikaExtractor.Extensions;
using TikaExtractor.Models;

namespace TikaExtractor.Services;

public class TikaParser
{
    private const string FILE_NAME_META_TAG = "resourceName";
    private const string CONTENT_META_TAG = "X-TIKA:content";
    private const string CONTENT_TYPE_META_TAG = "Content-Type";
    
    public IEnumerable<TextFile> ParseTextFiles(JsonDocument jsonDocument)
    {
        var rootElement = jsonDocument.RootElement;
        if (rootElement.IsArray())
        {
            for (var i = 0; i < rootElement.GetArrayLength(); i++)
            {
                var fileJsonElement = rootElement[i];
                yield return CreateTextFile(fileJsonElement);
            }
        }
        else
        {
            throw new JsonException("The root element of json is not an array");
        }
    }
    private TextFile CreateTextFile(JsonElement jsonElement)
    {
        var text = jsonElement.GetFieldValue(CONTENT_META_TAG);
        //todo: Вынести отсюда var language = await _tikaHttpClient.GetLanguageAsync(text);
        return new TextFile
        {
            Name = jsonElement.GetFieldValue(FILE_NAME_META_TAG),
            Text = text,
            ContentType = jsonElement.GetFieldValue(CONTENT_TYPE_META_TAG),
            //Language = language,
            ContentSize = text.Length
        };
    }

}