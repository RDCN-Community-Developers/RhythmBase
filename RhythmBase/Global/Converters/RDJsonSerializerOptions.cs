using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

internal record class RDJsonSerializerOptions
{
    public required JsonSerializerOptions JsonSerializerOptions { get; internal set; } = new JsonSerializerOptions
    {
        Converters =
        {
            new ColorConverter(),
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };
    public required LevelType Type { get; internal init; }
    public bool WriteAligned { get; internal init; }
}
