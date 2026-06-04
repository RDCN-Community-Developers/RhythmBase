using RhythmBase.Adofai.Events;
using System.Text.Json;
namespace RhythmBase.Adofai.Converters;

	internal class BaseEventConverter : MetadataJsonConverter<IBaseEvent>
	{
		public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
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
				e = ReadForwardEvent(ref reader, typeToConvert, options) ?? new ForwardEvent() { ActualType = type.ToString() ?? "" };
			else
				e = ConverterMap.GetConverter(typeEnum).ReadProperties(ref reader, options);
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.EndObject);
			reader.Read();
			return e;
		}
		public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
		{
			if(value is Events.IForwardEvent forwardEvent)
			{
				WriteForwardEvent(writer, forwardEvent);
				return;
			}
			ConverterMap.GetConverter(value.Type).WriteProperties(writer, value, options);
		}
		public static Events.IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
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
		public static void WriteForwardEvent(Utf8JsonWriter writer, Events.IForwardEvent value)
		{
			writer.WriteStartObject();
			writer.WriteString("eventType", value.ActualType);
			if (value is ForwardTileEvent tileEvent)
				writer.WriteNumber("floor", tileEvent._floor);
			value.Data.WriteTo(writer);
			writer.WriteEndObject();
		}

	}