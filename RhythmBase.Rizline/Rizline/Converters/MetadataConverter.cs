using RhythmBase.Rizline.Components;
using System.Text.Json;

namespace RhythmBase.Rizline.Converters;

[JsonConverterFor(typeof(Level))]
internal class MetadataConverter : MetadataJsonConverter<Level>
{
	public override Level? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		reader.Read();
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		Level level = new();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
		if (reader.ValueTextEquals("title"u8) && reader.Read())
			level.Title = reader.GetString() ?? "";
		else if (reader.ValueTextEquals("composer"u8) && reader.Read())
			level.Composer = reader.GetString() ?? "";
		else if (reader.ValueTextEquals("difficulty"u8) && reader.Read())
			level.Difficulty = reader.GetInt32();
		else if (reader.ValueTextEquals("level"u8) && reader.Read())
			level.Lv = reader.GetInt32();
		else if (reader.ValueTextEquals("maxHit"u8) && reader.Read())
			level.MaxHit = reader.GetInt32();
		else if (reader.ValueTextEquals("maxScore"u8) && reader.Read())
			level.MaxScore = reader.GetInt32();
		else if (reader.ValueTextEquals("previewTime"u8) && reader.Read())
			level.PreviewTime = TimeSpan.FromSeconds(reader.GetDouble());
		else
				reader.Skip();
		}
		return level;
	}

	public override void Write(Utf8JsonWriter writer, Level value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString("title", value.Title);
		writer.WriteString("composer", value.Composer);
		writer.WriteNumber("difficulty", value.Difficulty);
		writer.WriteNumber("level", value.Lv);
		writer.WriteNumber("maxHit", value.MaxHit);
		writer.WriteNumber("maxScore", value.MaxScore);
		writer.WriteNumber("previewTime", value.PreviewTime.TotalSeconds);
		writer.WriteEndObject();
	}
}
