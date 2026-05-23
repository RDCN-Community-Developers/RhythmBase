using RhythmBase.RhythmDoctor.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

[RDJsonConverterFor(typeof(RDColor))]
internal class ColorConverter : RDJsonConverter<RDColor>
{
    private enum ColorFormat
    {
        ArgbObject,
        RgbObject,
        ArgbHex,
        HashArgbHex,
        RgbaHex,
        HashRgbaHex,
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ColorFormat GetColorFormat(LevelType type)
    {
        return type switch
        {
            LevelType.RhythmDoctor or
            LevelType.Adofai
                => ColorFormat.RgbaHex,
            LevelType.BeatBlock
                => ColorFormat.RgbObject,
            _ => throw new JsonException($"Unexpected level type {type} when parsing RDColor."),
        };
    }
    public override RDColor Read(ref Utf8JsonReader reader, Type typeToConvert, RDJsonSerializerOptions options)
    {
        var tokenType = JsonException.ThrowIfNotMatch(reader, [JsonTokenType.String, JsonTokenType.Null, JsonTokenType.StartObject]);
        switch (GetColorFormat(options.Type))
        {
            case ColorFormat.RgbaHex or ColorFormat.HashRgbaHex:
                string? s = reader.GetString();
                if (string.IsNullOrEmpty(s)) return default;
                return RDColor.TryFromRgba(s!, out RDColor c) ? c : default;
            case ColorFormat.ArgbHex or ColorFormat.HashArgbHex:
                string? s2 = reader.GetString();
                if (string.IsNullOrEmpty(s2)) return default;
                return RDColor.TryFromArgb(s2!, out RDColor c2) ? c2 : default;
            case ColorFormat.RgbObject or ColorFormat.ArgbObject:
                RDColor color = default;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    { return color; }
                    JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
                    ReadOnlySpan<byte> propertyName = reader.ValueSpan;
                    reader.Read();
                    if (propertyName.SequenceEqual("r"u8) || propertyName.SequenceEqual("red"u8))
                        color.R = reader.GetByte();
                    else if (propertyName.SequenceEqual("g"u8) || propertyName.SequenceEqual("green"u8))
                        color.G = reader.GetByte();
                    else if (propertyName.SequenceEqual("b"u8) || propertyName.SequenceEqual("blue"u8))
                        color.B = reader.GetByte();
                    else if (propertyName.SequenceEqual("a"u8) || propertyName.SequenceEqual("alpha"u8))
                        color.A = reader.GetByte();
                    else
                        reader.Skip();
                }
                return color;
            default:
                throw new JsonException($"Unexpected token type {tokenType} when parsing RDColor.");
        }
    }

    public override void Write(Utf8JsonWriter writer, RDColor value, RDJsonSerializerOptions options)
    {
        switch (GetColorFormat(options.Type))
        {
            case ColorFormat.ArgbObject:
                writer.WriteStartObject();
                writer.WriteNumber("r", value.R);
                writer.WriteNumber("g", value.G);
                writer.WriteNumber("b", value.B);
                writer.WriteNumber("a", value.A);
                writer.WriteEndObject();
                break;
            case ColorFormat.RgbObject:
                writer.WriteStartObject();
                writer.WriteNumber("r", value.R);
                writer.WriteNumber("g", value.G);
                writer.WriteNumber("b", value.B);
                writer.WriteEndObject();
                break;
            case ColorFormat.RgbaHex:
                writer.WriteStringValue(value.ToString("rrggbbaa"));
                break;
            case ColorFormat.HashRgbaHex:
                writer.WriteStringValue(value.ToString("#rrggbbaa"));
                break;
            case ColorFormat.ArgbHex:
                writer.WriteStringValue(value.ToString("aarrggbb"));
                break;
            case ColorFormat.HashArgbHex:
                writer.WriteStringValue(value.ToString("#aarrggbb"));
                break;
        }
    }
}
