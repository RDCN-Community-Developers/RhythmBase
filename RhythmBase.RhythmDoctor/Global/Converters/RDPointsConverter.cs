using RhythmBase.Global.Components.Vector;
using System;
using System.Collections.Generic;
using System.Text;
using RhythmBase.Global.Extensions;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

[JsonConverterFor(typeof(PointE))]
internal class PointEConverter : JsonConverter<PointE>
{
    public override PointE Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new PointE(
            reader.Read() ?
            reader.TokenType == JsonTokenType.Number ? new Expression(reader.GetSingle()) :
            reader.TokenType == JsonTokenType.String ? new Expression(reader.GetString() ?? string.Empty) :
            (Expression?)null :
            null,
            reader.Read() ?
            reader.TokenType == JsonTokenType.Number ? new Expression(reader.GetSingle()) :
            reader.TokenType == JsonTokenType.String ? new Expression(reader.GetString() ?? string.Empty) :
            (Expression?)null :
            null
            );
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, PointE value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        if (value.X != null)
            if (value.X.Value.IsNumeric)
                writer.WriteNumberValue(value.X.Value.NumericValue);
            else
                writer.WriteStringValue(value.X.Value.ExpressionValue);
        else
            writer.WriteNullValue();
        if (value.Y != null)
            if (value.Y.Value.IsNumeric)
                writer.WriteNumberValue(value.Y.Value.NumericValue);
            else
                writer.WriteStringValue(value.Y.Value.ExpressionValue);
        else
            writer.WriteNullValue();
        writer.WriteEndArray();
    }
}
[JsonConverterFor(typeof(SizeE))]
internal class SizeEConverter : JsonConverter<SizeE>
{
    public override SizeE Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        var value = new SizeE(
            reader.Read() ?
            reader.TokenType == JsonTokenType.Number ? new Expression(reader.GetSingle()) :
            reader.TokenType == JsonTokenType.String ? new Expression(reader.GetString() ?? string.Empty) :
            (Expression?)null :
            null,
            reader.Read() ?
            reader.TokenType == JsonTokenType.Number ? new Expression(reader.GetSingle()) :
            reader.TokenType == JsonTokenType.String ? new Expression(reader.GetString() ?? string.Empty) :
            (Expression?)null :
            null);
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
        return value;
    }
    public override void Write(Utf8JsonWriter writer, SizeE value, JsonSerializerOptions serializer)
    {
        writer.WriteStartArray();
        if (value.Width != null)
            if (value.Width.Value.IsNumeric)
                writer.WriteNumberValue(value.Width.Value.NumericValue);
            else
                writer.WriteStringValue(value.Width.Value.ExpressionValue);
        else
            writer.WriteNullValue();
        if (value.Height != null)
            if (value.Height.Value.IsNumeric)
                writer.WriteNumberValue(value.Height.Value.NumericValue);
            else
                writer.WriteStringValue(value.Height.Value.ExpressionValue);
        else
            writer.WriteNullValue();
        writer.WriteEndArray();
    }
}