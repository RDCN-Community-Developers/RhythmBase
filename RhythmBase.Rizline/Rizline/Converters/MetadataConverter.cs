using RhythmBase.Rizline.Components;
using System.Text.Json;

namespace RhythmBase.Rizline.Converters;

[JsonConverterFor(typeof(Level))]
internal class MetadataConverter : MetadataJsonConverter<Level>
{
    public override Level? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
        Level level = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
            ReadOnlySpan<byte> propertyName = reader.ValueSpan;
            reader.Read();
            if (propertyName.SequenceEqual("title"u8))
                level.Title = reader.GetString() ?? "";
            else if (propertyName.SequenceEqual("composer"u8))
                level.Composer = reader.GetString() ?? "";
            else if (propertyName.SequenceEqual("difficulty"u8))
                level.Difficulty = reader.GetInt32();
            else if (propertyName.SequenceEqual("level"u8))
                level.Lv = reader.GetInt32();
            else if (propertyName.SequenceEqual("maxHit"u8))
                level.MaxHit = reader.GetInt32();
            else if (propertyName.SequenceEqual("maxScore"u8))
                level.MaxScore = reader.GetInt32();
            else if (propertyName.SequenceEqual("previewTime"u8))
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
