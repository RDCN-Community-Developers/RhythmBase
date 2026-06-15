using RhythmBase.Rizline.Components;
using RhythmBase.Rizline.Events;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Rizline.Converters
{
	[JsonConverterFor(typeof(Line))]
	internal class LineConverter : MetadataJsonConverter<Line>
	{
		private readonly InstanceConverter instanceConverter = new();
		public override Line? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
			Line line = new();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
			{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("linePoints"u8) && reader.Read())
			{
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					LinePoint e = EventConverterMap
						.GetConverter(EventType.LinePoint)
						.ReadProperties(ref reader, options)
						as LinePoint
						?? throw new JsonException("Failed to read a LinePoint event.");
					line.LinePoints.Add(e);
				}
			}
			else if (reader.ValueTextEquals("notes"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					//reader.Skip();// line.Notes.Add(TypeConverterRegistry.Read<Note>(ref reader, options));
					var note = instanceConverter.Read(ref reader, typeof(BaseNote), options);
					if (note is BaseNote n)
						line.Notes.Add(n);
					else
						throw new JsonException("Failed to read a Note event.");
				}
			}
			else if (reader.ValueTextEquals("judgeRingColor"u8) && reader.Read())
			{
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					JudgeRingColor e = EventConverterMap
					.GetConverter(EventType.JudgeRingColor)
					.ReadProperties(ref reader, options)
					as JudgeRingColor
					?? throw new JsonException("Failed to read a JudgeRingColor event.");
					line.JudgeRingColor.Add(e);
				}
			}
			else if (reader.ValueTextEquals("lineColor"u8) && reader.Read())
			{
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					LineColor e = EventConverterMap
					.GetConverter(EventType.LineColor)
					.ReadProperties(ref reader, options)
					as LineColor
					?? throw new JsonException("Failed to read a LineColor event.");
					line.LineColor.Add(e);
				}
			}
			else
					reader.Skip();
			}
			return line;
		}
		public override void Write(Utf8JsonWriter writer, Line value, MetadataJsonSerializerOptions options)
		{
			using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, options);
			writer.WriteStartObject();
			writer.WritePropertyName("linePoints");
			writer.WriteStartArray();
			noIndentScope.WriteNoIndentArrayTo(options, writer, value.LinePoints, instanceConverter.Write);
			writer.WriteEndArray();
			writer.WritePropertyName("notes");
			writer.WriteStartArray();
			noIndentScope.WriteNoIndentArrayTo(options, writer, value.Notes, instanceConverter.Write);
			writer.WriteEndArray();
			writer.WritePropertyName("judgeRingColor");
			writer.WriteStartArray();
			noIndentScope.WriteNoIndentArrayTo(options, writer, value.JudgeRingColor, instanceConverter.Write);
			writer.WriteEndArray();
			writer.WritePropertyName("lineColor");
			writer.WriteStartArray();
			noIndentScope.WriteNoIndentArrayTo(options, writer, value.LineColor, instanceConverter.Write);
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
	}
}
