using System.Text.Encodings.Web;
using System.Text.Json;

namespace RhythmBase.Global.Utils;

/// <summary>
/// Provides factory methods for creating <see cref="MetadataJsonSerializerOptions"/> configured for
/// reading or writing level files.
/// </summary>
public static class JsonSerializerOptionsUtils
{
    private static readonly JsonSerializerOptions options;
    static JsonSerializerOptionsUtils()
    {
        options = new()
        {
            AllowTrailingCommas = true,                
        };
    }
    /// <summary>
    /// Creates <see cref="MetadataJsonSerializerOptions"/> configured for deserializing a level file.
    /// </summary>
    /// <param name="settings">The level read settings.</param>
    /// <returns>A new <see cref="MetadataJsonSerializerOptions"/> instance.</returns>
    public static MetadataJsonSerializerOptions GetJsonSerializerOptionsForRead(LevelReadSettings settings)
    {
        MetadataJsonSerializerOptions options = new() { JsonSerializerOptions = new(JsonSerializerOptionsUtils.options) };
        return options;
    }
    /// <summary>
    /// Creates <see cref="MetadataJsonSerializerOptions"/> configured for serializing a level file.
    /// </summary>
    /// <param name="settings">The level write settings.</param>
    /// <returns>A new <see cref="MetadataJsonSerializerOptions"/> instance.</returns>
    public static MetadataJsonSerializerOptions GetJsonSerializerOptionsForWrite(LevelWriteSettings settings)
    {
        MetadataJsonSerializerOptions options = new() { JsonSerializerOptions = new(JsonSerializerOptionsUtils.options), WriteAligned = settings.WriteAligned };
        options.JsonSerializerOptions.WriteIndented = settings.WriteIndented;
        if (settings.EnableUnsafeRelaxedJsonEscaping)
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        return options;
    }
}
