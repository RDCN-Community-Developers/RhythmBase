using RhythmBase.Rizline.Components;
using System.Text.Json;

namespace RhythmBase.Rizline.Converters;

[JsonConverterFor(typeof(Theme))]
internal class ThemeConverter : MetadataJsonConverter<Theme>
{
	public override Theme Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		Theme theme = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("colorsList"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				reader.Read();
				theme.BackgroundColor = TypeConverterRegistry.Read<Color>(ref reader, options);
				reader.Read();
				theme.ObjectColor = TypeConverterRegistry.Read<Color>(ref reader, options);
				reader.Read();
				theme.UserInterfaceColor = TypeConverterRegistry.Read<Color>(ref reader, options);
				reader.Read();
			}
		}
		return theme;
	}

	public override void Write(Utf8JsonWriter writer, Theme value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteStartArray("colorsList"u8);
		TypeConverterRegistry.Write(writer, value.BackgroundColor, options);
		TypeConverterRegistry.Write(writer, value.ObjectColor, options);
		TypeConverterRegistry.Write(writer, value.UserInterfaceColor, options);
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
