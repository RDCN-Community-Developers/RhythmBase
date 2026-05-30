using RhythmBase.RhythmDoctor.Components;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(SoundSubTypeCollection))]
internal class SoundSubTypeCollectionConverter : JsonConverter<SoundSubTypeCollection>
{
	public override SoundSubTypeCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		SoundSubTypeCollection collection = [];
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
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("groupSubtype"u8) && Global.Converters.EnumConverter.TryParse(reader.ValueSpan, out SoundType result1))
					item.GroupSubtype = result1;
				else if (propertyName.SequenceEqual("used"u8))
					item.Used = reader.GetBoolean();
				else if (propertyName.SequenceEqual("filename"u8))
					item.Filename = reader.GetString() ?? string.Empty;
				else if (propertyName.SequenceEqual("volume"u8))
					item.Volume = reader.GetInt32();
				else if (propertyName.SequenceEqual("pitch"u8))
					item.Pitch = reader.GetInt32();
				else if (propertyName.SequenceEqual("pan"u8))
					item.Pan = reader.GetInt32();
				else if (propertyName.SequenceEqual("offset"u8))
					item.Offset = TimeSpan.FromMilliseconds(reader.GetSingle());
				else
				{
#if DEBUG
					Console.WriteLine(Encoding.UTF8.GetString([.. propertyName]));
#endif
					reader.Skip();
				}
			}
			collection._sounds.Add(item);
		}
		return collection;
	}

	public override void Write(Utf8JsonWriter writer, SoundSubTypeCollection value, JsonSerializerOptions options)
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
