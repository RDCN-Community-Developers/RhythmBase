using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(PlayerTypeGroup))]
internal class PlayerTypeGroupConverter : MetadataJsonConverter<PlayerTypeGroup>
{
	public override PlayerTypeGroup Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException($"Expected StartObject token, but got {reader.TokenType}.");
		PlayerTypeGroup group = new();
		if (options.Strictness == JsonStrictness.Strict)
			for (int i = 0; i < 16; i++)
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.String);
				group[i] = RhythmBase.Global.Converters.EnumConverter.TryParse(reader.ValueSpan, out PlayerType type) ? type : PlayerType.NoChange;
			}
		else
		{
			int i = 0;
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.String);
				group[i] = RhythmBase.Global.Converters.EnumConverter.TryParse(reader.ValueSpan, out PlayerType type) ? type : PlayerType.NoChange;
				i++;
			}
		}
		return reader.TokenType != JsonTokenType.EndArray
			? throw new JsonException($"Expected EndArray token, but got {reader.TokenType}.")
			: group;
	}

	public override void Write(Utf8JsonWriter writer, PlayerTypeGroup value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartArray();
		for (int i = 0; i < 16; i++)
			writer.WriteStringValue(value[i].ToEnumString());
		writer.WriteEndArray();
	}
}
