using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using System.Text;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

internal abstract class InstanceConverter<TEvent> : RhythmBase.Global.Converters.InstanceConverter<IBaseEvent> where TEvent : IBaseEvent, new()
{
	public sealed override IBaseEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
	{
		TEvent value = new();
		int bar = 1;
		float beat = 1;
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				value.TickTime = new(bar, beat);
				return value;
			}
			JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
			ReadOnlySpan<byte> propertyName = reader.ValueSpan;
			if (propertyName.IsEmpty)
				throw new JsonException("Property name cannot be null.");
			reader.Read();
			if (propertyName.SequenceEqual("bar"u8))
				bar = reader.GetInt32();
			else if (propertyName.SequenceEqual("beat"u8))
				beat = reader.GetSingle();
			else if (propertyName.SequenceEqual("type"u8))
				continue;
			else if (!Read(ref reader, propertyName, ref value, options))
			{
#if DEBUG
				//if (!(
				//	(value is FloatingText && propertyName.SequenceEqual("times"u8)) ||
				//	(value is FloatingText && propertyName.SequenceEqual("id"u8)) ||
				//	(value is AdvanceText && propertyName.SequenceEqual("id"u8))
				//	))
				//	Console.WriteLine($"The key {Encoding.UTF8.GetString([.. propertyName])} of {value.Type} not found.");
#endif
				value[
	Encoding.UTF8.GetString(propertyName)
	] = JsonElement.ParseValue(ref reader);
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
	protected virtual bool Read(ref Utf8JsonReader reader, ReadOnlySpan<byte> propertyName, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		bool result = true;
		if (propertyName.SequenceEqual("y"u8))
			value.Y = reader.GetInt32();
		else if (propertyName.SequenceEqual("tag"u8))
			value.Tag = reader.GetString() ?? string.Empty;
		else if (propertyName.SequenceEqual("runTag"u8))
			value.RunTag = reader.GetBoolean();
		else if (propertyName.SequenceEqual("if"u8))
			value.Condition = Condition.Deserialize(reader.GetString() ?? string.Empty);
		else if (propertyName.SequenceEqual("active"u8))
			value.Active = reader.GetBoolean();
		else
			result = false;
		return result;
	}
	protected virtual void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		(int bar, float beat) = value.TickTime;
		writer.WriteNumber("bar"u8, bar);
		if (value is not IBarBeginningEvent)
			writer.WriteNumber("beat"u8, beat);
		writer.WriteString("type"u8, value.Type.ToEnumString());
		if (value is not BaseDecorationAction)
			writer.WriteNumber("y"u8, value.Y);
		if (!string.IsNullOrEmpty(value.Tag))
			writer.WriteString("tag"u8, value.Tag);
		if (value.RunTag)
			writer.WriteBoolean("runTag"u8, true);
		if (value.Condition.HasValue)
			writer.WriteString("if"u8, value.Condition.Serialize());
		if (!value.Active)
			writer.WriteBoolean("active"u8, false);
	}
}