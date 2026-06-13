using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Room))]
internal class RoomConverter : MetadataJsonConverter<Room>
{
	public override Room Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
		Room result = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			if (reader.TokenType == JsonTokenType.EndArray)
				break;
			else
			{
				if (options.Strictness == JsonStrictness.Strict)
					result[reader.GetByte()] = true;
				else if (reader.TryGetByte(out byte index))
					result[index] = true;
			}
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
		return result;
	}

	public override void Write(Utf8JsonWriter writer, Room value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (byte item in value.Rooms)
			writer.WriteNumberValue(item);
		writer.WriteEndArray();
	}
}
[JsonConverterFor(typeof(SingleRoom))]
internal class SingleRoomConverter : MetadataJsonConverter<SingleRoom>
{
	public override SingleRoom Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
		reader.Read();
		SingleRoom result;
		if (options.Strictness == JsonStrictness.Strict || reader.TokenType == JsonTokenType.Number)
		{
			byte index = reader.GetByte();
			result = new(index);
			reader.Read();
		}
		else result = SingleRoom.Default;
		return result;
	}

	public override void Write(Utf8JsonWriter writer, SingleRoom value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartArray();
		writer.WriteNumberValue(value.Value);
		writer.WriteEndArray();
	}
}
