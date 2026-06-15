using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(FloatingTextAnchorStyle))]
internal class FloatingTextAnchorStylesConverter : JsonConverter<FloatingTextAnchorStyle>
{
	private static int count;
	public override FloatingTextAnchorStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.String);
		count++;
		FloatingTextAnchorStyle value = reader switch
		{
			var s when s.ValueTextEquals("UpperLeft"u8) => FloatingTextAnchorStyle.Upper | FloatingTextAnchorStyle.Left,
			var s when s.ValueTextEquals("UpperCenter"u8) => FloatingTextAnchorStyle.Upper,
			var s when s.ValueTextEquals("UpperRight"u8) => FloatingTextAnchorStyle.Upper | FloatingTextAnchorStyle.Right,
			var s when s.ValueTextEquals("MiddleLeft"u8) => FloatingTextAnchorStyle.Left,
			var s when s.ValueTextEquals("MiddleCenter"u8) => 0,
			var s when s.ValueTextEquals("MiddleRight"u8) => FloatingTextAnchorStyle.Right,
			var s when s.ValueTextEquals("LowerLeft"u8) => FloatingTextAnchorStyle.Lower | FloatingTextAnchorStyle.Left,
			var s when s.ValueTextEquals("LowerCenter"u8) => FloatingTextAnchorStyle.Lower,
			var s when s.ValueTextEquals("LowerRight"u8) => FloatingTextAnchorStyle.Lower | FloatingTextAnchorStyle.Right,
			_ => throw new JsonException($"Unknown {nameof(FloatingTextAnchorStyle)} value: {reader.GetString()}"),
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
