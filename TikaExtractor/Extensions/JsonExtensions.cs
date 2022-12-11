using System.Text.Json;

namespace TikaExtractor.Extensions;

public static class JsonExtensions
{
    public static bool IsUndefined(this JsonElement jsonElement)
    {
        return jsonElement.ValueKind == JsonValueKind.Undefined;
    }
}