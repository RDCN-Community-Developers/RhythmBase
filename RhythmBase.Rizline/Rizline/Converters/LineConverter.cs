using System;
using System.Collections.Generic;
using System.Text;
using RhythmBase.Rizline.Components;
using System.Text.Json.Serialization;
using System.Text.Json;
using RhythmBase.Rizline.Events;

namespace RhythmBase.Rizline.Converters
{
	[JsonConverterFor(typeof(Line))]
	internal class LineConverter : MetadataJsonConverter<Line>
	{
		public override Line? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
		{
			JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
			Line line = new();
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
					break;
				JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("linePoints"u8))
				{
					JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
						line.LinePoints.Add(ConverterHub.Read<LinePoint>(ref reader, options));
					reader.Read();
				}
				else if (propertyName.SequenceEqual("notes"u8))
				{
					JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
						line.Notes.Add(ConverterHub.Read<Note>(ref reader, options));
					reader.Read();
				}
				else if (propertyName.SequenceEqual("judgeRingColor"u8))
				{
					JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
						line.JudgeRingColor.Add(ConverterHub.Read<JudgeRingColor>(ref reader, options));
					reader.Read();
				}
				else if (propertyName.SequenceEqual("lineColor"u8))
				{
					JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
						line.LineColor.Add(ConverterHub.Read<LineColor>(ref reader, options));
					reader.Read();
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
			noIndentScope.WriteNoIndentArrayTo(options, writer, value.LinePoints, ConverterHub.Write);
			writer.WriteEndArray();
			writer.WritePropertyName("notes");
			writer.WriteStartArray();
			noIndentScope.WriteNoIndentArrayTo(options, writer, value.Notes, ConverterHub.Write);
			writer.WriteEndArray();
			writer.WritePropertyName("judgeRingColor");
			writer.WriteStartArray();
			noIndentScope.WriteNoIndentArrayTo(options, writer, value.JudgeRingColor, ConverterHub.Write);
			writer.WriteEndArray();
			writer.WritePropertyName("lineColor");
			writer.WriteStartArray();
			noIndentScope.WriteNoIndentArrayTo(options, writer, value.LineColor, ConverterHub.Write);
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
	}
}
