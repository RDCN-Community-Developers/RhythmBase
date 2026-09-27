using RhythmBase.Global.Serialization;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Serialization;

[JsonConverterFor(typeof(Audio))]
internal class AudioConverter : MetadataJsonConverter<Audio>
{
	public override Audio? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject, JsonTokenType.String);
		Audio audio = new();
		if (reader.TokenType == JsonTokenType.String)
		{
			audio.Filename = reader.GetString() ?? "";
			return audio;
		}
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("filename"u8) && reader.Read())
			{
				// Temporarily disabled: filename upgrades are now handled by RhythmDoctorUpgrader.
				audio.Filename = reader.GetString() ?? "";
			}
			else if (reader.ValueTextEquals("volume"u8) && reader.Read())
			{
				// Temporarily disabled: volume upgrades are now handled by RhythmDoctorUpgrader.
				audio.Volume = reader.GetInt32();
			}
			else if (reader.ValueTextEquals("pitch"u8) && reader.Read())
				audio.Pitch = reader.GetInt32();
			else if (reader.ValueTextEquals("pan"u8) && reader.Read())
				audio.Pan = reader.GetInt32();
			else if (reader.ValueTextEquals("offset"u8) && reader.Read())
				audio.Offset = TimeSpan.FromMilliseconds(reader.GetDouble());
			else
				throw new JsonException($"Unknown property: {reader.GetString()}");
		}
		return audio;
	}

	public override void Write(Utf8JsonWriter writer, Audio value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString("filename"u8, value.Filename);
		if (value.Volume != 100)
			writer.WriteNumber("volume"u8, value.Volume);
		if (value.Pitch != 100)
			writer.WriteNumber("pitch"u8, value.Pitch);
		if (value.Pan != 0)
			writer.WriteNumber("pan"u8, value.Pan);
		if (value.Offset != TimeSpan.Zero)
			writer.WriteNumber("offset"u8, value.Offset.TotalMilliseconds);
		writer.WriteEndObject();
	}
}
