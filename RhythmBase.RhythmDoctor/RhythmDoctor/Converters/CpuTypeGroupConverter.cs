using RhythmBase.Global.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Characters[]))]
internal class CpuTypeGroupConverter : MetadataJsonConverter<Characters[]>
{
	public override Characters[] Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		if(reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException($"Expected StartArray token, but got {reader.TokenType}.");
		reader.Read();
		Characters[] group = new Characters[16];
		for(int i = 0; i < 16; i++)
		{
			if(reader.TokenType == JsonTokenType.String)
			{
				group[i] = EnumConverter.TryParse(reader.ValueSpan, out Characters type) ? type : Characters.None;
			}
			else
			{
				throw new JsonException($"Expected String token, but got {reader.TokenType}.");
			}
			reader.Read();
		}
		return group;
	}

	public override void Write(Utf8JsonWriter writer, Characters[] value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartArray();
		for(int i = 0; i < 16; i++)
			writer.WriteStringValue(value[i].ToString());
		writer.WriteEndArray();
	}
}
