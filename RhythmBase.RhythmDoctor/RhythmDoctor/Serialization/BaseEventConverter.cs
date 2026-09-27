using RhythmBase.Global.Serialization;
using RhythmBase.RhythmDoctor.Events;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Serialization;

internal class BaseEventConverter : MetadataJsonConverter<IBaseEvent>
{
	public override bool CanConvert(Type typeToConvert)
	{
		return Type.IsAssignableFrom(typeToConvert);
	}
	public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		string? type = null;
		Utf8JsonReader checkpoint = reader;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				if (reader.ValueTextEquals("type"u8))
				{
					reader.Read();
					type = reader.GetString();
					// Temporarily disabled: migrated to RhythmDoctorUpgrader.
					//if (type == ReorderDeoraionOldName)
					//	type = ReorderDeoraionNewName;
					break;
				}
				else
				{
					reader.Skip();
				}
			}
		}
		reader = checkpoint; IBaseEvent e;
		if (string.IsNullOrEmpty(type))
			throw new JsonException("Event type is missing.");
		if (!Enum.TryParse(type, true, out EventType typeEnum))
			e = ReadForwardEvent(ref reader, type!) ?? throw new JsonException("Unknown event type and failed to parse.");
		else
			{
			e = EventConverterMap.GetConverter(typeEnum).ReadProperties(ref reader, options); }
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndObject);
		return e;
	}
	public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		if (value is Events.IForwardEvent ce)
		{
			WriteForwardEvent(writer, ce, options.ConditionIdOffset);
			return;
		}
		else
		{
			EventConverterMap.GetConverter(value.Type).WriteProperties(writer, value, options);
		}
	}
	public static Events.IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader, string type)
	{
		JsonDocument doc = JsonDocument.ParseValue(ref reader);
		JsonElement root = doc.RootElement;

		// 判断属性
		bool hasRow = false, hasTarget = false;
		foreach (JsonProperty prop in root.EnumerateObject())
		{
			if (prop.NameEquals("row"))
				hasRow = true;
			else if (prop.NameEquals("target"))
				hasTarget = true;
		}
		return
			hasRow ? new ForwardRowEvent(doc) { ActualType = type } :
			hasTarget ? new ForwardDecorationEvent(doc) { ActualType = type } :
			new ForwardEvent(doc) { ActualType = type };
	}

	public static void WriteForwardEvent(Utf8JsonWriter writer, Events.IForwardEvent value, int conditionIdOffset)
	{
		(int bar, float beat) = value.TickTime;
		writer.WriteStartObject();
		if (!string.IsNullOrEmpty(value.ActualType))
			writer.WriteString("type", value.ActualType);
		writer.WriteNumber("bar", bar);
		writer.WriteNumber("beat", beat);
		if (value is ForwardRowEvent rowEvent)
			writer.WriteNumber("row", rowEvent.Row);
		else if (value is ForwardDecorationEvent decorationEvent)
			writer.WriteString("target", decorationEvent.Target);
		if (!string.IsNullOrEmpty(value.Tag))
			writer.WriteString("tag", value.Tag);
		if (value.RunTag)
			writer.WriteBoolean("runTag", value.RunTag);
		if (!value.Active)
			writer.WriteBoolean("active", value.Active);
		if (!value.Condition.IsEmpty)
			writer.WriteString("if", value.Condition.Serialize(conditionIdOffset));
		if (value.Y != 0)
			writer.WriteNumber("y", value.Y);

		foreach (KeyValuePair<string, JsonElement> kv in ((BaseEvent)value)._extraData)
		{
			writer.WritePropertyName(kv.Key);
			kv.Value.WriteTo(writer);
		}
		writer.WriteEndObject();
	}
}
