using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Room))]
internal class RoomConverter : JsonConverter<Room>
{
    private readonly byte[] buffer = new byte[RoomCount];
    private int index = 0;
    public override Room Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        while (reader.Read())
            if (reader.TokenType == JsonTokenType.EndArray)
                break;
            else
                buffer[index++] = reader.GetByte();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        Room result = new();
        for (int i = 0; i < index; i++)
            result[buffer[i]] = true;
        index = 0;
        return result;
    }

    public override void Write(Utf8JsonWriter writer, Room value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (byte item in value.Rooms)
            writer.WriteNumberValue(item);
        writer.WriteEndArray();
    }
}
[JsonConverterFor(typeof(SingleRoom))]
internal class SingleRoomConverter : JsonConverter<SingleRoom>
{
    public override SingleRoom Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
        reader.Read();
        byte index = reader.GetByte();
        SingleRoom result = new(index);
        reader.Read();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndArray);
        return result;
    }

    public override void Write(Utf8JsonWriter writer, SingleRoom value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Value);
        writer.WriteEndArray();
    }
}
