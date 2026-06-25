using RhythmBase.BeatBlock.Components;
using RhythmBase.BeatBlock.Events;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Converters;
internal abstract class EventInstanceConverterBase : Global.Converters.MemberConverter<IBaseEvent> { }
internal abstract class MemberConverter<TEvent> : EventInstanceConverterBase where TEvent : IBaseEvent, new()
{
	public sealed override IBaseEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
	{
		TEvent value = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			var checkpoint = reader;
			if (reader.ValueTextEquals("type"u8) && reader.Read())
				continue;
			else if (!Read(ref reader, ref value, options))
			{
				reader = checkpoint;
				string fieldName = reader.GetString()!;
				reader.Read();
				JsonElement fieldValue = JsonElement.ParseValue(ref reader);

				if (UnhandledFieldRegistry.TryHandle(ref value, fieldName, fieldValue, (int)value.Type))
					continue;
				if (options.TryHandleUser(ref value, fieldName, fieldValue, (int)value.Type))
					continue;

				value[fieldName] = fieldValue;
			}
		}
		return value;
	}
	public sealed override void WriteProperties(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		TEvent v = (TEvent)value;
		writer.WriteStartObject();
		writer.WriteString("type"u8, v.Type.ToEnumUtf8String());
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
		if (reader.ValueTextEquals("time"u8) && reader.Read())
			value.TickTime = new TickTime(reader.GetSingle());
		else if (reader.ValueTextEquals("angle"u8) && reader.Read())
			value.Angle = reader.GetSingle();
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
		if (value.Variant is not null)
			writer.WriteString("variant"u8, value.Variant);
		writer.WriteNumber("time"u8, value.TickTime.Tick);
		writer.WriteNumber("angle"u8, value.Angle);
		if (value.Order is int valueNotNull)
			writer.WriteNumber("order"u8, valueNotNull);
	}
}