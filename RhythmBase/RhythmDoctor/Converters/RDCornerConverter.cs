using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[global::RhythmBase.Global.Converters.RDJsonConverterFor(typeof(Corner))]
internal class RDCornerConverter : RDJsonConverter<Corner>
{
    public override Corner Read(ref Utf8JsonReader reader, Type typeToConvert, RDJsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray, JsonTokenType.Null]);
        reader.Read();
        Corner corner = new()
        {
            LeftBottom = ReadOneCorner(ref reader, options),
            RightBottom = ReadOneCorner(ref reader, options),
            LeftTop = ReadOneCorner(ref reader, options),
            RightTop = ReadOneCorner(ref reader, options)
        };
        if (reader.TokenType != JsonTokenType.EndArray)
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return corner;
    }
    private static RDPoint? ReadOneCorner(ref Utf8JsonReader reader, RDJsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray, JsonTokenType.Null]);
        if (reader.TokenType == JsonTokenType.Null)
        {
            reader.Read();
            return null;
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            var value = ConverterHub.Read<RDPoint>(ref reader, options);
            reader.Read();
            return value;
        }
        else { return null; }
    }
    public override void Write(Utf8JsonWriter writer, Corner value, RDJsonSerializerOptions options)
    {
        writer.WriteStartArray();
        WriteOneCorner(writer, value.LeftBottom, options);
        WriteOneCorner(writer, value.RightBottom, options);
        WriteOneCorner(writer, value.LeftTop, options);
        WriteOneCorner(writer, value.RightTop, options);
        writer.WriteEndArray();
    }
    private static void WriteOneCorner(Utf8JsonWriter writer, RDPoint? point, RDJsonSerializerOptions options)
    {
        if (point is RDPoint p)
            ConverterHub.Write(writer, p, options);
        else
            writer.WriteNullValue();
    }
}