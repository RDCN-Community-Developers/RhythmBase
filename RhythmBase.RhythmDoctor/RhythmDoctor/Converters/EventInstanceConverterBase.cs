using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

internal abstract class MemberConverterBase : Global.Converters.MemberConverter<IBaseEvent> { }
internal abstract class MemberConverter<TEvent> : MemberConverterBase where TEvent : IBaseEvent, new()
{
	public sealed override IBaseEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
	{
		TEvent value = new();
		int bar = 1;
		float beat = 1;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("bar"u8) && reader.Read())
			{
				bar = reader.GetInt32();
			}
			else if (reader.ValueTextEquals("beat"u8) && reader.Read())
			{
				beat = reader.GetSingle();
			}
			else if (reader.ValueTextEquals("type"u8) && reader.Read())
			{
				continue;
			}
			else
			{
				var span = reader.ValueSpan;
				var seq = reader.ValueSequence;
				bool hasSeq = reader.HasValueSequence;
				if (!Read(ref reader, ref value, options))
				{
					byte[] propertyNameArray = hasSeq ? seq.ToArray() : span.ToArray();
					string propertyName = Encoding.UTF8.GetString(propertyNameArray);
					// 忽略的属性，主要是为了兼容旧版本的事件数据
					if ((
						(value is FloatingText && propertyName == "times") ||
						(value is FloatingText && propertyName == "id") ||
						(value is AdvanceText && propertyName == "id") ||
						(value is PlaySound && propertyName == "isCustom") ||
						(value is MaskRoom && propertyName == "contentMode") ||
						(value is MaskRoom && propertyName == "rooms") ||
						(value is TintRows && propertyName == "borderOpacity") || // 1
						(value is TintRows && propertyName == "tintOpacity") || // 1
						(value is TintRows && propertyName == "effectSound") || 
						(value is Tint && propertyName == "borderOpacity") || // 1
						(value is Tint && propertyName == "tintOpacity") || // 1
						(value is PaintHands && propertyName == "borderOpacity") || // 1
						(value is PaintHands && propertyName == "tintOpacity") || // 1
						(value is NewWindowDance && propertyName == "rooms") ||
						(value is NewWindowDance && propertyName == "usePosition") ||
						(value is AddOneshotBeat && propertyName == "squareSound") ||
						(value is SetGameSound && propertyName == "sounds") ||
						(value is SetClapSounds && propertyName == "p1Used") ||
						(value is SetClapSounds && propertyName == "p2Used") ||
						(value is SetClapSounds && propertyName == "cpuUsed") ||
						(value is SetVFXPreset && propertyName == "xySpeed")

						))
					{
						reader.Skip();
						continue;
					}
					value[propertyName] = JsonElement.ParseValue(ref reader);
#if DEBUG
					Console.WriteLine($"{options.Version}\t| {value.Type}\t| {propertyName} => ({value[propertyName].ValueKind}){value[propertyName]}");
#endif
				}
			}
		}
		value.TickTime = new(bar, beat);
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
		if (reader.ValueTextEquals("y"u8) && reader.Read())
			value.Y = reader.GetInt32();
		else if (reader.ValueTextEquals("tag"u8) && reader.Read())
			value.Tag = reader.GetString() ?? string.Empty;
		else if (reader.ValueTextEquals("runTag"u8) && reader.Read())
			value.RunTag = reader.GetBoolean();
		else if (reader.ValueTextEquals("if"u8) && reader.Read())
			value.Condition = Condition.Deserialize(reader.GetString() ?? string.Empty);
		else if (reader.ValueTextEquals("active"u8) && reader.Read())
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