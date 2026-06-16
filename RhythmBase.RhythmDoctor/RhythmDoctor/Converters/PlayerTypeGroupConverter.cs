using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(PlayerTypeGroup))]
internal class PlayerTypeGroupConverter : MetadataJsonConverter<PlayerTypeGroup>
{
	public override PlayerTypeGroup Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
		PlayerTypeGroup group = new();
		if (options.Strictness == JsonStrictness.Strict)
		{
			for (int i = 0; i < 16; i++)
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.String);
				group[i] = RhythmBase.Global.Converters.EnumConverter.TryParse(ref reader, out PlayerType type) ? type : PlayerType.NoChange;
			}
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
		}
		else
		{
			int i = 0;
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.String);
				group[i++] = RhythmBase.Global.Converters.EnumConverter.TryParse(ref reader, out PlayerType type) ? type : PlayerType.NoChange;
			}
		}
		return group;
	}

	public override void Write(Utf8JsonWriter writer, PlayerTypeGroup value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartArray();
		for (int i = 0; i < 16; i++)
			writer.WriteStringValue(value[i].ToEnumString());
		writer.WriteEndArray();
	}
}
