using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Serialization;

[JsonConverterFor(typeof(PointE))]
internal class PointEConverter : JsonConverter<PointE>
{
	public override PointE Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
		var value = new PointE(
				reader.Read() ?
				reader.TokenType == JsonTokenType.Number ? new Expression(reader.GetSingle()) :
				reader.TokenType == JsonTokenType.String ? new Expression(reader.GetString()?.TrimStart('{').TrimEnd('}') ?? string.Empty) :
				(Expression?)null :
				null,
				reader.Read() ?
				reader.TokenType == JsonTokenType.Number ? new Expression(reader.GetSingle()) :
				reader.TokenType == JsonTokenType.String ? new Expression(reader.GetString()?.TrimStart('{').TrimEnd('}') ?? string.Empty) :
				(Expression?)null :
				null
				);
		reader.Read();
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
		return value;
	}
	public override void Write(Utf8JsonWriter writer, PointE value, JsonSerializerOptions serializer)
	{
		writer.WriteStartArray();
		if (value.X is Expression x)
			TypeConverterRegistry.Write(writer, x, new() { JsonSerializerOptions = serializer });
		else
			writer.WriteNullValue();
		if (value.Y is Expression y)
			TypeConverterRegistry.Write(writer, y, new() { JsonSerializerOptions = serializer });
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
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
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
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
		return value;
	}
	public override void Write(Utf8JsonWriter writer, SizeE value, JsonSerializerOptions serializer)
	{
		writer.WriteStartArray();
		if (value.Width is Expression w)
			TypeConverterRegistry.Write(writer, w, new() { JsonSerializerOptions = serializer });
		else
			writer.WriteNullValue();
		if (value.Height is Expression h)
			TypeConverterRegistry.Write(writer, h, new() { JsonSerializerOptions = serializer });
		else
			writer.WriteNullValue();
		writer.WriteEndArray();
	}
}