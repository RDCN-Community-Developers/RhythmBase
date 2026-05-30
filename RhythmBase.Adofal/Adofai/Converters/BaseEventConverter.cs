using RhythmBase.Adofai.Events;
using System.Text.Json;
using static RhythmBase.Adofai.Utils.EventTypeUtils;
namespace RhythmBase.Adofai.Converters;

	internal class BaseEventConverter : RDJsonConverter<IBaseEvent>
	{
		public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
		{
			JsonException.ThrowIfNotMatch(reader,	[JsonTokenType.StartObject]);
			ReadOnlySpan<byte> type = default;
			Utf8JsonReader checkpoint = reader;
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
					break;
				if (reader.TokenType == JsonTokenType.PropertyName)
				{
					if (reader.ValueSpan.SequenceEqual("eventType"u8))
					{
						reader.Read();
						type = reader.ValueSpan;
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
				e = ReadForwardEvent(ref reader, typeToConvert, options) ?? new ForwardEvent() { ActureType = type.ToString() ?? "" };
			else
				e = converters[typeEnum].ReadProperties(ref reader, options);
			JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndObject]);
			reader.Read();
			return e;
		}
		public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
		{
			if(value is IForwardEvent forwardEvent)
			{
				WriteForwardEvent(writer, forwardEvent);
				return;
			}
			converters[value.Type].WriteProperties(writer, value, options);
		}
		public IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
		{
			using JsonDocument doc = JsonDocument.ParseValue(ref reader);
			JsonElement root = doc.RootElement;

			// 判断属性
			bool isTile = false;
			foreach (JsonProperty prop in root.EnumerateObject())
			{
				if (prop.NameEquals("floor"))
					isTile = true;
			}
			return isTile ? new ForwardTileEvent(doc) : new ForwardEvent(doc);
		}
		public static void WriteForwardEvent(Utf8JsonWriter writer, IForwardEvent value)
		{
			writer.WriteStartObject();
			writer.WriteString("eventType", value.ActureType);
			if (value is ForwardTileEvent tileEvent)
				writer.WriteNumber("floor", tileEvent._floor);
			value.Data.WriteTo(writer);
			writer.WriteEndObject();
		}

	}