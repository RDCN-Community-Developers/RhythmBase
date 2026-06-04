using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.Global.Converters;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Corner))]
internal class RDCornerConverter : MetadataJsonConverter<Corner>
{
    public override Corner Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray, JsonTokenType.Null);
        reader.Read();
        Corner corner = new()
        {
            LeftBottom = ReadOneCorner(ref reader, options),
            RightBottom = ReadOneCorner(ref reader, options),
            LeftTop = ReadOneCorner(ref reader, options),
            RightTop = ReadOneCorner(ref reader, options)
        };
        if (reader.TokenType != JsonTokenType.EndArray)
            JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return corner;
    }
    private static Point? ReadOneCorner(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray, JsonTokenType.Null);
        if (reader.TokenType == JsonTokenType.Null)
        {
            reader.Read();
            return null;
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            var value = ConverterHub.Read<Point>(ref reader, options);
            reader.Read();
            return value;
        }
        else { return null; }
    }
    public override void Write(Utf8JsonWriter writer, Corner value, MetadataJsonSerializerOptions options)
    {
        writer.WriteStartArray();
        WriteOneCorner(writer, value.LeftBottom, options);
        WriteOneCorner(writer, value.RightBottom, options);
        WriteOneCorner(writer, value.LeftTop, options);
        WriteOneCorner(writer, value.RightTop, options);
        writer.WriteEndArray();
    }
    private static void WriteOneCorner(Utf8JsonWriter writer, Point? point, MetadataJsonSerializerOptions options)
    {
        if (point is Point p)
            ConverterHub.Write(writer, p, options);
        else
            writer.WriteNullValue();
    }
}