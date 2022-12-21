using System.Text.Json;

namespace TextsExtractor.Extensions;

public static class JsonExtensions
{
    public static bool IsUndefined(this JsonElement jsonElement)
    {
        return jsonElement.ValueKind == JsonValueKind.Undefined;
    }
    
    public static bool IsArray(this JsonElement jsonElement)
    {
        return jsonElement.ValueKind == JsonValueKind.Array;
    }
    
    public static string GetFieldValue(this JsonElement element, string fieldName)
    {
        element.TryGetProperty(fieldName, out var fieldElement);
        return fieldElement.IsUndefined() ? String.Empty : fieldElement.ToString().Trim();
    }
}