using System.Text.Json;
using System.Text.Json.Serialization;
using RhythmBase.Adofai.Components;

namespace RhythmBase.Adofai.Converters;

[JsonConverterFor(typeof(TileReference))]
internal class TileReferenceConverter : JsonConverter<TileReference>
{
    public override TileReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.Number]);
        int offset = reader.GetInt32();
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.String]);
        ReadOnlySpan<byte> typeSpan = reader.ValueSpan;
        if (!EnumConverter.TryParse(typeSpan, out RelativeType type))
            throw new JsonException($"Invalid RelativeType value: {typeSpan.ToString()}");
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return new TileReference
        {
            Offset = offset,
            Type = type
        };
    }
    public override void Write(Utf8JsonWriter writer, TileReference value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Offset);
        writer.WriteStringValue(value.Type.ToEnumString());
        writer.WriteEndArray();
    }
}
