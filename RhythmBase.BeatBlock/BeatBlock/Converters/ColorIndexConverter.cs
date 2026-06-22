using RhythmBase.BeatBlock.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.BeatBlock.Converters;

[JsonConverterFor(typeof(ColorIndex))]
internal class ColorIndexConverter : JsonConverter<ColorIndex>
{
	public override ColorIndex Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.Number);
		return new ColorIndex(reader.GetByte());
	}

	public override void Write(Utf8JsonWriter writer, ColorIndex value, JsonSerializerOptions options)
	{
		writer.WriteNumberValue(value);
	}
}
[JsonConverterFor(typeof(ColorIndexOrDefault))]
internal class ColorIndexOrDefaultConverter : JsonConverter<ColorIndexOrDefault>
{
	public override ColorIndexOrDefault Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.Number);
		sbyte value = reader.GetSByte();
		if(value == -1)
			return ColorIndexOrDefault.Default;
		return new ColorIndex((byte)value);
	}
	public override void Write(Utf8JsonWriter writer, ColorIndexOrDefault value, JsonSerializerOptions options)
	{
			writer.WriteNumberValue(value == ColorIndexOrDefault.Default ? -1 : value.Value);
	}
}