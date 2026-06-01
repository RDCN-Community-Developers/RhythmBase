using RhythmBase.Global.Components.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace RhythmBase.Global.Converters;

public abstract class RDPointsConverter<T> : JsonConverter<T> where T : struct, IVector
{
    public override bool CanConvert(Type objectType) => typeof(IVector).IsAssignableFrom(objectType);
}
[JsonConverterFor(typeof(Point))]
public class RDPointConverter : JsonConverter<Point>
{
    public override Point Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new Point(
                reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetSingle() : null,
                reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetSingle() : null);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        if (value.X is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.X.Value);
        if (value.Y is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Y.Value);
        writer.WriteEndArray();
    }
}
[JsonConverterFor(typeof(PointNI))]
public class RDPointNIConverter : JsonConverter<PointNI>
{
    public override PointNI Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new PointNI(
            reader.Read() ? reader.GetInt32() : 0,
            reader.Read() ? reader.GetInt32() : 0);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, PointNI value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.X);
        writer.WriteNumberValue(value.Y);
        writer.WriteEndArray();
    }
}
[JsonConverterFor(typeof(PointN))]
public class RDPointNConverter : JsonConverter<PointN>
{
    public override PointN Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new PointN(
            reader.Read() ? reader.GetSingle() : 0,
            reader.Read() ? reader.GetSingle() : 0);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, PointN value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.X);
        writer.WriteNumberValue(value.Y);
        writer.WriteEndArray();
    }
}
[JsonConverterFor(typeof(PointI))]
public class RDPointIConverter : JsonConverter<PointI>
{
    public override PointI Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new PointI(
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : null,
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : null);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, PointI value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        if (value.X is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.X.Value);
        if (value.Y is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Y.Value);
        writer.WriteEndArray();
    }
}
[JsonConverterFor(typeof(SizeNI))]
public class RDSizeNIConverter : JsonConverter<SizeNI>
{
    public override SizeNI Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new SizeNI(
            reader.Read() ? reader.GetInt32() : 0,
            reader.Read() ? reader.GetInt32() : 0);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, SizeNI value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Width);
        writer.WriteNumberValue(value.Height);
        writer.WriteEndArray();
    }
}
[JsonConverterFor(typeof(SizeN))]
public class RDSizeNConverter : JsonConverter<SizeN>
{
    public override SizeN Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new SizeN(
            reader.Read() ? reader.GetSingle() : 0,
            reader.Read() ? reader.GetSingle() : 0);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, SizeN value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Width);
        writer.WriteNumberValue(value.Height);
        writer.WriteEndArray();
    }
}
[JsonConverterFor(typeof(SizeI))]
public class RDSizeIConverter : JsonConverter<SizeI>
{
    public override SizeI Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new SizeI(
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : null,
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : null);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, SizeI value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        if (value.Width is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Width.Value);
        if (value.Height is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Height.Value);
        writer.WriteEndArray();
    }
}
[JsonConverterFor(typeof(Size))]
public class RDSizeConverter : JsonConverter<Size>
{
    public override Size Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new Size(
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetSingle() : null,
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetSingle() : null);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, Size value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        if (value.Width is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Width.Value);
        if (value.Height is null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Height.Value);
        writer.WriteEndArray();
    }
}