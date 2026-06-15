using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using RhythmBase.Global.Converters;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Bookmark))]
internal class BookmarkConverter() : MetadataJsonConverter<Bookmark>
{
	public override void Write(Utf8JsonWriter writer, Bookmark value, MetadataJsonSerializerOptions serializer)
	{
		(int bar, float beat) = value.Beat;
		writer.WriteStartObject();
		writer.WriteNumber("bar", bar);
		writer.WriteNumber("beat", beat);
		writer.WriteNumber("color", (int)value.Color);
		writer.WriteEndObject();
	}
	public override Bookmark Read(ref Utf8JsonReader reader, Type objectType, MetadataJsonSerializerOptions serializer)
	{
		int bar = 1;
		float beat = 1;
		Bookmark.BookmarkColors color = Bookmark.BookmarkColors.Blue;
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				if (reader.ValueTextEquals("bar"u8))
				{
					reader.Read();
					bar = reader.GetInt32();
				}
				else if (reader.ValueTextEquals("beat"u8))
				{
					reader.Read();
					beat = reader.GetSingle();
				}
				else if (reader.ValueTextEquals("color"u8))
				{
					reader.Read();
					color = (Bookmark.BookmarkColors)reader.GetInt32();
				}
				else
				{
					reader.Skip();
				}
			}
		}
		return new Bookmark
		{
			Beat = new(bar, beat),
			Color = color,
		};
	}
}
