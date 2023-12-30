using System.Text.Json;

namespace Meshmakers.Common.Shared;

/// <summary>
/// Generic object extensions.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static string Serialize(this object o)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Serialize(o, serializerOptions);
    }
}
