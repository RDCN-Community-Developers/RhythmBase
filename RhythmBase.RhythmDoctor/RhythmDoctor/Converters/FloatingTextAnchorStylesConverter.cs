using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(FloatingTextAnchorStyle))]
internal class FloatingTextAnchorStylesConverter : JsonConverter<FloatingTextAnchorStyle>
{
	public override FloatingTextAnchorStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"Expected String token, but got {reader.TokenType}.");
		FloatingTextAnchorStyle value = reader.ValueSpan switch
		{
			var s when s.SequenceEqual("UpperLeft"u8) => FloatingTextAnchorStyle.Upper | FloatingTextAnchorStyle.Left,
			var s when s.SequenceEqual("UpperCenter"u8) => FloatingTextAnchorStyle.Upper,
			var s when s.SequenceEqual("UpperRight"u8) => FloatingTextAnchorStyle.Upper | FloatingTextAnchorStyle.Right,
			var s when s.SequenceEqual("MiddleLeft"u8) => FloatingTextAnchorStyle.Left,
			var s when s.SequenceEqual("MiddleCenter"u8) => 0,
			var s when s.SequenceEqual("MiddleRight"u8) => FloatingTextAnchorStyle.Right,
			var s when s.SequenceEqual("LowerLeft"u8) => FloatingTextAnchorStyle.Lower | FloatingTextAnchorStyle.Left,
			var s when s.SequenceEqual("LowerCenter"u8) => FloatingTextAnchorStyle.Lower,
			var s when s.SequenceEqual("LowerRight"u8) => FloatingTextAnchorStyle.Lower | FloatingTextAnchorStyle.Right,
			_ => throw new JsonException($"Unknown FloatingTextAnchorStyles value: {reader.GetString()}"),
		};
		return value;
	}

	public override void Write(Utf8JsonWriter writer, FloatingTextAnchorStyle value, JsonSerializerOptions options)
	{
		ReadOnlySpan<byte> s = value switch
		{
			FloatingTextAnchorStyle.Upper | FloatingTextAnchorStyle.Left => "UpperLeft"u8,
			FloatingTextAnchorStyle.Upper => "UpperCenter"u8,
			FloatingTextAnchorStyle.Upper | FloatingTextAnchorStyle.Right => "UpperRight"u8,
			FloatingTextAnchorStyle.Left => "MiddleLeft"u8,
			0 => "MiddleCenter"u8,
			FloatingTextAnchorStyle.Right => "MiddleRight"u8,
			FloatingTextAnchorStyle.Lower | FloatingTextAnchorStyle.Left => "LowerLeft"u8,
			FloatingTextAnchorStyle.Lower => "LowerCenter"u8,
			FloatingTextAnchorStyle.Lower | FloatingTextAnchorStyle.Right => "LowerRight"u8,
			_ => throw new JsonException($"Unknown FloatingTextAnchorStyles value: {value}"),
		};
		writer.WriteStringValue(s);
	}
}
