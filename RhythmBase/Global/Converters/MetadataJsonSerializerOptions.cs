using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

public record class MetadataJsonSerializerOptions
{
    public required JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };
    public required LevelType Type { get; init; }
    public bool WriteAligned { get; init; }
    public bool WriteIndented { get => JsonSerializerOptions.WriteIndented; init => JsonSerializerOptions.WriteIndented = value; }
}
