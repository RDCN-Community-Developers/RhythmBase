using System.Text.Encodings.Web;
using System.Text.Json;

namespace RhythmBase.Global.Utils;

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
    public static MetadataJsonSerializerOptions GetJsonSerializerOptionsForRead(LevelReadSettings settings)
    {
        MetadataJsonSerializerOptions options = new() { JsonSerializerOptions = new(JsonSerializerOptionsUtils.options) };
        return options;
    }
    public static MetadataJsonSerializerOptions GetJsonSerializerOptionsForWrite(LevelWriteSettings settings)
    {
        MetadataJsonSerializerOptions options = new() { JsonSerializerOptions = new(JsonSerializerOptionsUtils.options), WriteAligned = settings.WriteAligned };
        options.JsonSerializerOptions.WriteIndented = settings.WriteIndented;
        if (settings.EnableUnsafeRelaxedJsonEscaping)
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        return options;
    }
}
