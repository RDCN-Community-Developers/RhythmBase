using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(RoomHeight))]
internal class RoomHeightConverter : JsonConverter<RoomHeight>
{
	public override RoomHeight Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
		RoomHeight height = new();
		for (int i = 0; i < 4; i++)
		{
			reader.Read();
			JsonException.ThrowIfNotMatch(reader, [JsonTokenType.Number]);
			height[i] = reader.GetInt32();
		}
		reader.Read();
		JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndArray]);
		return height;
	}

	public override void Write(Utf8JsonWriter writer, RoomHeight value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		for (int i = 0; i < 4; i++)
			writer.WriteNumberValue(value[i]);
		writer.WriteEndArray();
	}
}
