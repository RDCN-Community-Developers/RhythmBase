using RhythmBase.Global.Converters;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(GameCharacter[]))]
internal class CpuTypeGroupConverter : MetadataJsonConverter<GameCharacter[]>
{
	public override GameCharacter[] Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		if(reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException($"Expected StartArray token, but got {reader.TokenType}.");
		reader.Read();
		GameCharacter[] group = new GameCharacter[16];
		for(int i = 0; i < 16; i++)
		{
			if(reader.TokenType == JsonTokenType.String)
			{
				group[i] = EnumConverter.TryParse(ref reader, out GameCharacter type) ? type : GameCharacter.None;
			}
			else
			{
				throw new JsonException($"Expected String token, but got {reader.TokenType}.");
			}
			reader.Read();
		}
		return group;
	}

	public override void Write(Utf8JsonWriter writer, GameCharacter[] value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartArray();
		for(int i = 0; i < 16; i++)
			writer.WriteStringValue(value[i].ToString());
		writer.WriteEndArray();
	}
}
