using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(PlayerTypeGroup))]
internal class PlayerTypeGroupConverter : JsonConverter<PlayerTypeGroup>
{
	public override PlayerTypeGroup Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException($"Expected StartObject token, but got {reader.TokenType}.");
		reader.Read();
		PlayerTypeGroup group = new();
		for (int i = 0; i < 16; i++)
		{
			if (reader.TokenType == JsonTokenType.String)
			{
				group[i] = RhythmBase.Global.Converters.EnumConverter.TryParse(reader.ValueSpan, out PlayerType type) ? type : PlayerType.NoChange;
			}
			else
			{
				throw new JsonException($"Expected String token, but got {reader.TokenType}.");
			}
			reader.Read();
		}
		return reader.TokenType != JsonTokenType.EndArray
			? throw new JsonException($"Expected EndArray token, but got {reader.TokenType}.")
			: group;
	}

	public override void Write(Utf8JsonWriter writer, PlayerTypeGroup value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		for (int i = 0; i < 16; i++)
			writer.WriteStringValue(value[i].ToEnumString());
		writer.WriteEndArray();
	}
}
