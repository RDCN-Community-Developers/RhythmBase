using RhythmBase.RhythmDoctor.Components;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(SoundCollection))]
internal class SoundCollectionConverter : JsonConverter<SoundCollection>
{
	public override SoundCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		SoundCollection collection = [];
		List<SoundSubType> sounds = [];
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected StartArray token");
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
				break;
			if (reader.TokenType != JsonTokenType.StartObject)
				throw new JsonException("Expected StartObject token");
			SoundSubType item = new();
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
					break;
				if (reader.TokenType != JsonTokenType.PropertyName)
					throw new JsonException("Expected PropertyName token");
				if (reader.ValueTextEquals("groupSubtype"u8) && reader.Read() && Global.Converters.EnumConverter.TryParse(ref reader, out SoundType result1))
					item.GroupSubtype = result1;
				else if (reader.ValueTextEquals("used"u8) && reader.Read())
					item.Used = reader.GetBoolean();
				else if (reader.ValueTextEquals("filename"u8) && reader.Read())
					item.Filename = reader.GetString() ?? string.Empty;
				else if (reader.ValueTextEquals("volume"u8) && reader.Read())
					item.Volume = reader.GetInt32();
				else if (reader.ValueTextEquals("pitch"u8) && reader.Read())
					item.Pitch = reader.GetInt32();
				else if (reader.ValueTextEquals("pan"u8) && reader.Read())
					item.Pan = reader.GetInt32();
				else if (reader.ValueTextEquals("offset"u8) && reader.Read())
					item.Offset = TimeSpan.FromMilliseconds(reader.GetSingle());
				else
				{
#if DEBUG
					Console.WriteLine($"Found unknown property '{reader.GetString()}' in {nameof(SoundCollection)}");
#endif
					reader.Skip();
				}
			}
			collection._sounds.Add(item);
		}
		return collection;
	}

	public override void Write(Utf8JsonWriter writer, SoundCollection value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (SoundSubType? item in value._sounds)
		{
			writer.WriteStartObject();
			writer.WriteString("groupSubtype"u8, item.GroupSubtype.ToString());
			writer.WriteBoolean("used"u8, item.Used);
			writer.WriteString("filename"u8, item.Filename);
			if(item.Volume != 100)
				writer.WriteNumber("volume"u8, item.Volume);
			if (item.Pitch != 100)
				writer.WriteNumber("pitch"u8, item.Pitch);
			if(item.Pan != 0)
				writer.WriteNumber("pan"u8, item.Pan);
			if(item.Offset != TimeSpan.Zero)
				writer.WriteNumber("offset"u8, item.Offset.TotalMilliseconds);
			writer.WriteEndObject();
		}
		writer.WriteEndArray();
	}
}
