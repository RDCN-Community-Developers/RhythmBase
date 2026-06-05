using RhythmBase.Rizline.Events;
using System.Text.Json;

namespace RhythmBase.Rizline.Converters;

internal class InstanceConverter : MetadataJsonConverter<IBaseEvent>
{
	public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		int type = -1;
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
					type = reader.GetInt32();
					break;
				}
				else
				{
					reader.Skip();
				}
			}
		}
		reader = checkpoint; IBaseEvent e;
		switch(type)
		{
			case -1:
				throw new NotImplementedException();
			case 0 or 1 or 2:
				e = EventConverterMap.GetConverter((EventType)type).ReadProperties(ref reader, options);
				break;
			default:
				throw new JsonException($"Unknown note type: {type}");
		}
		return e;
	}

	public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		EventConverterMap.GetConverter(value.Type).WriteProperties(writer, value, options);
	}
}
internal abstract class MemberConverter : Global.Converters.MemberConverter<IBaseEvent> { }
internal abstract class MemberConverter<TEvent> : MemberConverter where TEvent : IBaseEvent, new()
{
	public override IBaseEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
	{
		TEvent value = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			ReadOnlySpan<byte> propertyName = reader.ValueSpan;
			reader.Read();
			if (propertyName.SequenceEqual("type"u8))
				continue;
			else if (!Read(ref reader, propertyName, ref value, options))
			{
				//value[System.Text.Encoding.UTF8.GetString(propertyName)] = JsonDocument.ParseValue(ref reader).RootElement.Clone();
				reader.Skip();
			}
		}
		return value;
	}
	public override void WriteProperties(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		TEvent v = (TEvent)value;
		writer.WriteStartObject();
		Write(writer, ref v, options);
		writer.WriteEndObject();
	}
	protected virtual bool Read(ref Utf8JsonReader reader, ReadOnlySpan<byte> propertyName, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		bool result = true;
		if (propertyName.SequenceEqual("time"u8))
			value.TickTime = new(reader.GetSingle());
		else
			result = false;
		return result;
	}
	protected virtual void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		float time = value.TickTime.Tick;
		writer.WriteNumber("time", time);
	}
}