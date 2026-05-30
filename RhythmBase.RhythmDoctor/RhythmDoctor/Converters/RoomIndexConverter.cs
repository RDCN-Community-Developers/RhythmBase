using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(RoomIndex))]
internal class RoomIndexConverter : JsonConverter<RoomIndex>
{
	public override RoomIndex Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.Number)
			throw new JsonException($"Expected Number token, but got {reader.TokenType}.");
		byte index = reader.GetByte();
		return index > 4 ? throw new JsonException($"Room index must be between 0 and 4, but got {index}.") : (RoomIndex)(1 << index);
	}

	public override void Write(Utf8JsonWriter writer, RoomIndex value, JsonSerializerOptions options)
	{
		writer.WriteNumberValue(value switch
		{
			RoomIndex.Room1 => 0,
			RoomIndex.Room2 => 1,
			RoomIndex.Room3 => 2,
			RoomIndex.Room4 => 3,
			_ => throw new JsonException($"Cannot convert {value} to room index."),
		});
	}
}
