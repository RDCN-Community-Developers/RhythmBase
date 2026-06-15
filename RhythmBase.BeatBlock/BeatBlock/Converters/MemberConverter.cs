using RhythmBase.BeatBlock.Events;
using System.Text;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Converters;

internal class BaseEventConverter : MetadataJsonConverter<IBaseEvent>
{
	private LevelReadSettings _rs = new();
	private LevelWriteSettings _ws = new();
	public BaseEventConverter WithReadSettings(LevelReadSettings settings)
	{
		_rs = settings;
		return this;
	}
	public BaseEventConverter WithWriteSettings(LevelWriteSettings settings)
	{
		_ws = settings;
		return this;
	}
	public override bool CanConvert(Type typeToConvert)
	{
		return Type.IsAssignableFrom(typeToConvert);
	}
	public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		string? type = null;

		Utf8JsonReader checkpoint = reader;
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				if (reader.ValueTextEquals("type"u8))
				{
					reader.Read();
					type = reader.GetString();
					break;
				}
				else
				{
					reader.Skip();
				}
			}
		}
		reader = checkpoint; IBaseEvent e;
		if (!EnumConverter.TryParse(type, out EventType typeEnum))
			e = ReadForwardEvent(ref reader) ?? new ForwardEvent() { ActualType = type ?? "" };
		else
			e = EventConverterMap.GetConverter(typeEnum).WithReadSettings(_rs).ReadProperties(ref reader, options);
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndObject);
		return e;
	}
	public static Events.IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader)
	{
		JsonDocument doc = JsonDocument.ParseValue(ref reader);
		//JsonElement root = doc.RootElement;

		return new ForwardEvent(doc);
	}

	public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		if (value is Events.IForwardEvent ce)
		{
			WriteForwardEvent(writer, ce);
			return;
		}
		else
		{
			EventConverterMap.GetConverter(value.Type).WithWriteSettings(_ws).WriteProperties(writer, value, options);
		}
	}

	private static void WriteForwardEvent(Utf8JsonWriter writer, Events.IForwardEvent value)
	{
		writer.WriteStartObject();
		if (!string.IsNullOrEmpty(value.ActualType))
			writer.WriteString("type"u8, value.ActualType);
		foreach (KeyValuePair<string, JsonElement> kv in ((BaseEvent)(IBaseEvent)value)._extraData)
		{
			{
				writer.WritePropertyName(kv.Key);
				kv.Value.WriteTo(writer);
			}
			writer.WriteEndObject();
		}
	}

}
internal abstract class EventInstanceConverterBase : Global.Converters.MemberConverter<IBaseEvent> { }
internal abstract class MemberConverter<TEvent> : EventInstanceConverterBase where TEvent : IBaseEvent, new()
{
	public sealed override IBaseEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
	{
		TEvent value = new();
		float time = 0;
		float angle = 0;
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				value.Time = time;
				value.Angle = angle;
				return value;
			}
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("time"u8))
			{
				reader.Read();
				time = reader.GetSingle();
			}
			else if (reader.ValueTextEquals("angle"u8))
			{
				reader.Read();
				angle = reader.GetSingle();
			}
			else if (reader.ValueTextEquals("type"u8))
			{
				reader.Read();
				continue;
			}
		else
		{
			string pn = reader.GetString()!;
			if (!Read(ref reader, ref value, options))
			{
#if DEBUG
				//if (!(
				//	(value is FloatingText && propertyName.SequenceEqual("times"u8)) ||
				//	(value is FloatingText && propertyName.SequenceEqual("id"u8)) ||
				//	(value is AdvanceText && propertyName.SequenceEqual("id"u8))
				//	))
				//	Console.WriteLine($"The key {Encoding.UTF8.GetString([.. propertyName])} of {value.Type} not found.");
#endif
				reader.Read();
				value[pn] = JsonElement.ParseValue(ref reader);
			}
		}
		}
		return value;
	}
	public sealed override void WriteProperties(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		TEvent v = (TEvent)value;
		writer.WriteStartObject();
		Write(writer, ref v, options);
		foreach (KeyValuePair<string, JsonElement> kv in ((BaseEvent)(IBaseEvent)v)._extraData)
		{
			writer.WritePropertyName(kv.Key);
			writer.WriteRawValue(kv.Value.GetRawText());
		}
		writer.WriteEndObject();
	}
	protected virtual bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		bool result = true;
		if (reader.ValueTextEquals("angle"u8) && reader.Read())
			value.Angle = reader.GetSingle();
		else if (reader.ValueTextEquals("time"u8) && reader.Read())
			value.Time = reader.GetSingle();
		else if (reader.ValueTextEquals("variant"u8) && reader.Read())
			value.Variant = reader.GetString();
		else if (reader.ValueTextEquals("order"u8) && reader.Read())
			value.Order = reader.GetInt32();
		else
			result = false;
		return result;
	}
	protected virtual void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		writer.WriteNumber("angle"u8, value.Angle);
		writer.WriteNumber("time"u8, value.Time);
		if (value.Variant is not null)
			writer.WriteString("variant"u8, value.Variant);
		if (value.Order is int valueNotNull)
			writer.WriteNumber("order"u8, valueNotNull);
	}
}