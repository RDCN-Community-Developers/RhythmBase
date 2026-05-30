using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Order))]
internal class OrderConverter : JsonConverter<Order>
{
	public override Order Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException($"Expected StartArray token, but got {reader.TokenType}.");
		List<int> orders = [];
		while (reader.Read()) {
			if (reader.TokenType == JsonTokenType.EndArray)
				break;
			if (reader.TokenType != JsonTokenType.Number)
				throw new JsonException($"Expected Number token, but got {reader.TokenType}.");
			if (!reader.TryGetByte(out byte b))
				throw new JsonException($"Expected a byte value between 0 and 255, but got {reader.GetDouble()}.");
			orders.Add(b);
        }
		return  new([.. orders]);
	}

	public override void Write(Utf8JsonWriter writer, Order value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (byte b in value.Indices)
			writer.WriteNumberValue(b);
		writer.WriteEndArray();
	}
}
