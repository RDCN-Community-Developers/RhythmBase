using RhythmBase.BeatBlock.Events;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Serialization;

internal class BaseEventConverter : MetadataJsonConverter<IBaseEvent>
{
	public override bool CanConvert(Type typeToConvert)
	{
		return Type.IsAssignableFrom(typeToConvert);
	}
	public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);

		Utf8JsonReader checkpoint = reader;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("type"u8) && reader.Read())
				break;
			else
				reader.Skip();
		}
		IBaseEvent e;
		// upgrate to the latest version

		if (EnumConverter.TryParse(ref reader, out EventType typeEnum))
			e = EventConverterMap.GetConverter(typeEnum).ReadProperties(ref checkpoint, options);
		else
			// Temporarily disabled: missing-type inference is replaced by BeatBlockUpgrader.
			e = ReadForwardEvent(ref checkpoint) ?? throw new JsonException("Unknown event type and failed to parse.");
		JsonException.ThrowIfNotMatch(ref checkpoint, JsonTokenType.EndObject);
		reader = checkpoint;
		return e;
	}
	public static Events.IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader)
	{
		JsonDocument doc = JsonDocument.ParseValue(ref reader);
		JsonElement root = doc.RootElement;

		return new ForwardEvent(doc);
	}

	public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		if (value is Events.IForwardEvent ce)
			WriteForwardEvent(writer, ce);
		else
			EventConverterMap.GetConverter(value.Type).WriteProperties(writer, value, options);
	}

	private static void WriteForwardEvent(Utf8JsonWriter writer, Events.IForwardEvent value)
	{
		writer.WriteStartObject();
		if (!string.IsNullOrEmpty(value.ActualType))
			writer.WriteString("type"u8, value.ActualType);
		foreach (KeyValuePair<string, JsonElement> kv in ((BaseEvent)(IBaseEvent)value)._extraData)
		{
			writer.WritePropertyName(kv.Key);
			kv.Value.WriteTo(writer);
		}
		writer.WriteEndObject();
	}

}
