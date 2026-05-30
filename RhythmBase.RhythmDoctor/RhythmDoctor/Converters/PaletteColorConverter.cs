using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(PaletteColor))]
internal class PaletteColorConverter : JsonConverter<PaletteColor>
{
	public override PaletteColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		ReadOnlySpan<byte> t = reader.ValueSpan;
		return t.Length == 0
			? default
			: (t is [0x70, 0x61, 0x6c, ..])
			? new(int.Parse(t[3..]))
			: new(Color.FromRgba(t));
	}

	public override void Write(Utf8JsonWriter writer, PaletteColor value, JsonSerializerOptions options)
	{
		if (value.EnablePanel)
		{
			writer.WriteStringValue($"pal{value.PaletteIndex}");
		}
		else
		{
			writer.WriteStringValue(value.Value.ToString("RRGGBB"));
		}
	}
}
