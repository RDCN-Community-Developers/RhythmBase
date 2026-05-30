using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Audio[]))]
internal class CountingSoundCollectionConverter : JsonConverter<Audio[]>
{
	private static readonly AudioConverter audioConverter = new();
	public override Audio[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException($"Unexpected token parsing CountingSoundCollection. Expected StartArray or Null, got {reader.TokenType}.");
		List<Audio> audios = [];
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				return [.. audios];
			}
			Audio? audio = audioConverter.Read(ref reader, typeof(Audio), options) ?? throw new JsonException("Null audio in CountingSoundCollection.");
			audios.Add(audio);
		}
		throw new JsonException("Unexpected end of JSON while reading CountingSoundCollection.");
	}
	public override void Write(Utf8JsonWriter writer, Audio[] value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		for (int i = 0; i < 7; i++)
			audioConverter.Write(writer, value[i], options);
		writer.WriteEndArray();
	}
}
