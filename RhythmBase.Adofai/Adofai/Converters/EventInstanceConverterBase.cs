using RhythmBase.Adofai.Events;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Adofai.Converters;
internal abstract class EventMemberConverterBase: Global.Converters.MemberConverter<IBaseEvent>{}
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
		foreach (KeyValuePair<string,JsonElement> kv in ((BaseEvent)(IBaseEvent)v)._extraData)
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
		if (value is BaseTileEvent bte && bte._floor >= 0)
			writer.WriteNumber("floor"u8, bte._floor);
		writer.WriteString("eventType"u8, value.Type.ToEnumString());
		if (value is BaseTaggedTileEvent btta)
		{
			if (btta.AngleOffset != 0)
				writer.WriteNumber("angleOffset"u8, btta.AngleOffset);
			if (!string.IsNullOrEmpty(btta.EventTag))
				writer.WriteString("eventTag"u8, btta.EventTag);
		}
	}
}