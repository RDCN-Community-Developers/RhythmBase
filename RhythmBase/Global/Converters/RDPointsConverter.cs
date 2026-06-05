using RhythmBase.Global.Components.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace RhythmBase.Global.Converters;

/// <summary>
/// Abstract base class for JSON converters that handle <see cref="IVector"/> types as JSON arrays.
/// </summary>
/// <typeparam name="T">The vector type being converted.</typeparam>
public abstract class RDPointsConverter<T> : JsonConverter<T> where T : struct, IVector
{
    /// <inheritdoc/>
    public override bool CanConvert(Type objectType) => typeof(IVector).IsAssignableFrom(objectType);
}
/// <summary>
/// Converts a nullable <see cref="Point"/> (float, nullable) to and from a JSON array.
/// </summary>
[JsonConverterFor(typeof(Point))]
public class RDPointConverter : JsonConverter<Point>
{
    /// <inheritdoc/>
    public override Point Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        var value = new Point(
                reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetSingle() : null,
                reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetSingle() : null);
        reader.Read();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return value;
    }
    /// <inheritdoc/>
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
/// <summary>
/// Converts a non-nullable integer <see cref="PointNI"/> to and from a JSON array.
/// </summary>
[JsonConverterFor(typeof(PointNI))]
public class RDPointNIConverter : JsonConverter<PointNI>
{
    /// <inheritdoc/>
    public override PointNI Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        var value = new PointNI(
            reader.Read() ? reader.GetInt32() : 0,
            reader.Read() ? reader.GetInt32() : 0);
        reader.Read();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return value;
    }
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, PointNI value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.X);
        writer.WriteNumberValue(value.Y);
        writer.WriteEndArray();
    }
}
/// <summary>
/// Converts a non-nullable float <see cref="PointN"/> to and from a JSON array.
/// </summary>
[JsonConverterFor(typeof(PointN))]
public class RDPointNConverter : JsonConverter<PointN>
{
    /// <inheritdoc/>
    public override PointN Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        var value = new PointN(
            reader.Read() ? reader.GetSingle() : 0,
            reader.Read() ? reader.GetSingle() : 0);
        reader.Read();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return value;
    }
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, PointN value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.X);
        writer.WriteNumberValue(value.Y);
        writer.WriteEndArray();
    }
}
/// <summary>
/// Converts a nullable integer <see cref="PointI"/> to and from a JSON array.
/// </summary>
[JsonConverterFor(typeof(PointI))]
public class RDPointIConverter : JsonConverter<PointI>
{
    /// <inheritdoc/>
    public override PointI Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        var value = new PointI(
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : null,
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : null);
        reader.Read();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return value;
    }
    /// <inheritdoc/>
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
/// <summary>
/// Converts a non-nullable integer <see cref="SizeNI"/> to and from a JSON array.
/// </summary>
[JsonConverterFor(typeof(SizeNI))]
public class RDSizeNIConverter : JsonConverter<SizeNI>
{
    /// <inheritdoc/>
    public override SizeNI Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        var value = new SizeNI(
            reader.Read() ? reader.GetInt32() : 0,
            reader.Read() ? reader.GetInt32() : 0);
        reader.Read();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return value;
    }
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, SizeNI value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Width);
        writer.WriteNumberValue(value.Height);
        writer.WriteEndArray();
    }
}
/// <summary>
/// Converts a non-nullable float <see cref="SizeN"/> to and from a JSON array.
/// </summary>
[JsonConverterFor(typeof(SizeN))]
public class RDSizeNConverter : JsonConverter<SizeN>
{
    /// <inheritdoc/>
    public override SizeN Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        var value = new SizeN(
            reader.Read() ? reader.GetSingle() : 0,
            reader.Read() ? reader.GetSingle() : 0);
        reader.Read();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return value;
    }
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, SizeN value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Width);
        writer.WriteNumberValue(value.Height);
        writer.WriteEndArray();
    }
}
/// <summary>
/// Converts a nullable integer <see cref="SizeI"/> to and from a JSON array.
/// </summary>
[JsonConverterFor(typeof(SizeI))]
public class RDSizeIConverter : JsonConverter<SizeI>
{
    /// <inheritdoc/>
    public override SizeI Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        var value = new SizeI(
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : null,
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : null);
        reader.Read();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return value;
    }
    /// <inheritdoc/>
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
/// <summary>
/// Converts a nullable float <see cref="Size"/> to and from a JSON array.
/// </summary>
[JsonConverterFor(typeof(Size))]
public class RDSizeConverter : JsonConverter<Size>
{
    /// <inheritdoc/>
    public override Size Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        var value = new Size(
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetSingle() : null,
            reader.Read() && reader.TokenType == JsonTokenType.Number ? reader.GetSingle() : null);
        reader.Read();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return value;
    }
    /// <inheritdoc/>
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