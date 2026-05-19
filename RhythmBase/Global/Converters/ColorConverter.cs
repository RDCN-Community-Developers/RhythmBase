using RhythmBase.RhythmDoctor.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

internal record class RDJsonSerializerOptions
{
    public JsonSerializerOptions JsonSerializerOptions { get; internal set; } = new JsonSerializerOptions
    {
        Converters =
        {
            new ColorConverter(),
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };
}
internal abstract class RDJsonConverter<T> : JsonConverter<T>
{
    public RDJsonSerializerOptions JsonSerializerOptions { get; internal set; }
    public RDJsonConverter<T> WithOptions(JsonSerializerOptions options)
    {
        this.JsonSerializerOptions.JsonSerializerOptions = options;
        return this;
    }
    public RDJsonConverter<T> WithOptions(RDJsonSerializerOptions options)
    {
        this.JsonSerializerOptions = options;
        return this;
    }
    public virtual T? Read(ref Utf8JsonReader reader, Type typeToConvert, RDJsonSerializerOptions options)
    {
        return Read(ref reader, typeToConvert, options.JsonSerializerOptions);
    }
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    { 
    }
}

[RDJsonConverterFor(typeof(RDColor))]
internal class ColorConverter : JsonConverter<RDColor>
{
    public override RDColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var tokenType = JsonException.ThrowIfNotMatch(reader, [JsonTokenType.String, JsonTokenType.Null, JsonTokenType.StartObject]);
        switch (tokenType)
        {
            case JsonTokenType.String:
            case JsonTokenType.Null:
                string? s = reader.GetString();
                if (string.IsNullOrEmpty(s)) return default;
                return RDColor.TryFromRgba(s!, out RDColor c) ? c : default;
            case JsonTokenType.StartObject:
                RDColor color = default;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {  return default; }
                    JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
                    reader.Read();
                    if (reader.ValueSpan.SequenceEqual("r"u8) || reader.ValueSpan.SequenceEqual("red"u8))
                        color.R = reader.GetByte();
                    else if (reader.ValueSpan.SequenceEqual("g"u8) || reader.ValueSpan.SequenceEqual("green"u8))
                        color.G = reader.GetByte();
                    else if (reader.ValueSpan.SequenceEqual("b"u8) || reader.ValueSpan.SequenceEqual("blue"u8))
                        color.B = reader.GetByte();
                    else if (reader.ValueSpan.SequenceEqual("a"u8) || reader.ValueSpan.SequenceEqual("alpha"u8))
                        color.A = reader.GetByte();
                    else
                        reader.Skip();
                }
                return color;
            default:
                throw new JsonException($"Unexpected token type {tokenType} when parsing RDColor.");
        }
    }

    public override void Write(Utf8JsonWriter writer, RDColor value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("rrggbbaa"));
    }
}
