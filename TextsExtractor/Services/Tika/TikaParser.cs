using System.Text.Json;
using TextsExtractor.Models;
using TextsExtractor.Extensions;

namespace TextsExtractor.Services.Tika;

public static class TikaParser
{
    private const string CONTENT_META_TAG = "X-TIKA:content";
    private const string CONTENT_TYPE_META_TAG = "Content-Type";
    
    public static List<TextFile> ParseTextFiles(JsonDocument jsonDocument)
    {
        var textFiles = new List<TextFile>();
        var rootElement = jsonDocument.RootElement;
        
        if (!rootElement.IsArray()) 
            throw new JsonException("The root element of json is not an array");
        
        for (var i = 0; i < rootElement.GetArrayLength(); i++)
        {
            var fileJsonElement = rootElement[i];
            var textFile = CreateTextFile(fileJsonElement);
            if(!TextFileIsEmpty(textFile))
                textFiles.Add(textFile);
        }
        return textFiles;

    }
    private static TextFile CreateTextFile(JsonElement jsonElement)
    {
        var text = jsonElement.GetFieldValue(CONTENT_META_TAG);
        return new TextFile
        {
            Text = text,
            ContentType = jsonElement.GetFieldValue(CONTENT_TYPE_META_TAG),
            ContentSize = text.Length
        };
    }
    private static bool TextFileIsEmpty(TextFile textFile)
    {
        return textFile.ContentSize < 150;
    }

}