using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Converters;

[RDJsonConverterFor(typeof(Bookmark))]
internal class BookmarkConverter() : RDJsonConverter<Bookmark>
{
	public override void Write(Utf8JsonWriter writer, Bookmark value, RDJsonSerializerOptions serializer)
	{
		(int bar, float beat) = value.Beat;
		writer.WriteStartObject();
		writer.WriteNumber("bar", bar);
		writer.WriteNumber("beat", beat);
		writer.WriteNumber("color", (int)value.Color);
		writer.WriteEndObject();
	}
	public override Bookmark Read(ref Utf8JsonReader reader, Type objectType, RDJsonSerializerOptions serializer)
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
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				switch (propertyName)
				{
					case var p when p.SequenceEqual("bar"u8):
						bar = reader.GetInt32();
						break;
					case var p when p.SequenceEqual("beat"u8):
						beat = reader.GetSingle();
						break;
					case var p when p.SequenceEqual("color"u8):
						color = (Bookmark.BookmarkColors)reader.GetInt32();
						break;
					default:
						reader.Skip();
						break;
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
