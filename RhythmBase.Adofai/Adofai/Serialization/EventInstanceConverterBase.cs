using RhythmBase.Adofai.Events;
using System.Text.Json;

namespace RhythmBase.Adofai.Serialization;

internal abstract class EventMemberConverterBase: Global.Serialization.MemberConverter<IBaseEvent>{}
internal abstract class MemberConverter<TEvent> : EventMemberConverterBase where TEvent : IBaseEvent, new()
{
	public sealed override IBaseEvent ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
	{
		TEvent value = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("eventType"u8))
			{
				reader.Read();
				continue;
			}
			string pn = reader.GetString() ?? "";
			if (!Read(ref reader, ref value, options))
			{
#if DEBUG
				if (!(false
					))
					Console.WriteLine($"The key {pn} of {value.Type} not found.");
#endif
				reader.Read();
				value[pn] = JsonElement.ParseValue(ref reader);
			}
		}
		return value;
	}
	public sealed override void WriteProperties(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
	{
		TEvent v = (TEvent)value;
		writer.WriteStartObject();
		Write(writer, ref v, options);
		Dictionary<string, JsonElement> extra = v switch
		{
			BaseEvent be => be._extraData,
			BaseTileEvent bte => bte._extraData,
			_ => []
		};
		foreach (KeyValuePair<string,JsonElement> kv in extra)
		{
			writer.WritePropertyName(kv.Key);
			writer.WriteRawValue(kv.Value.GetRawText());
		}
		writer.WriteEndObject();
	}
	protected virtual bool Read(ref Utf8JsonReader reader, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		bool result = false;
		return result;
	}
	protected virtual void Write(Utf8JsonWriter writer, ref TEvent value, MetadataJsonSerializerOptions options)
	{
		int floor = value switch
		{
			BaseEvent be => be._floor,
			BaseTileEvent bte => bte._floor,
			_ => -1
		};
		if (floor >= 0)
			writer.WriteNumber("floor"u8, floor);
		writer.WriteString("eventType"u8, value.Type.ToEnumString());
		if (value is BaseEvent angleEvent)
		{
			if (angleEvent.AngleOffset != 0f)
				writer.WriteNumber("angleOffset"u8, angleEvent.AngleOffset);
			if (angleEvent is BaseTaggedTileEvent tagged && !string.IsNullOrEmpty(tagged.EventTag))
				writer.WriteString("eventTag"u8, tagged.EventTag);
		}
	}
}