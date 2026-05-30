using RhythmBase.Rizline.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Rizline.Converters;

[JsonConverterFor(typeof(Theme))]
internal class ThemeConverter : MetadataJsonConverter<Theme>
{
    public override Theme Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
        reader.Read();
        ReadOnlySpan<byte> propertyName = reader.ValueSpan;
        Theme theme = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
            if (propertyName.SequenceEqual("colorsList"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                reader.Read();
                theme.BackgroundColor = ConverterHub.Read<Color>(ref reader, options);
                reader.Read();
                theme.ObjectColor = ConverterHub.Read<Color>(ref reader, options);
                reader.Read();
                theme.UserInterfaceColor = ConverterHub.Read<Color>(ref reader, options);
                reader.Read();
            }
        }
        return theme;
    }

    public override void Write(Utf8JsonWriter writer, Theme value, MetadataJsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteStartArray("colorsList"u8);
        ConverterHub.Write(writer, value.BackgroundColor, options);
        ConverterHub.Write(writer, value.ObjectColor, options);
        ConverterHub.Write(writer, value.UserInterfaceColor, options);
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
