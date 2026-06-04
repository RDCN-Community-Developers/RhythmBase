using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

public abstract class ColorConverter : JsonConverter<Color>
{
    public enum ColorFormat
    {
        ArgbObject,
        RgbObject,
        ArgbHex,
        HashArgbHex,
        RgbaHex,
        HashRgbaHex,
    }
    public static ColorConverter GetInstance(ColorFormat format)
    {
        return format switch
        {
            ColorFormat.ArgbObject => new ArgbObject(),
            ColorFormat.RgbObject => new RgbObject(),
            ColorFormat.ArgbHex => new ArgbHex(),
            ColorFormat.HashArgbHex => new HashArgbHex(),
            ColorFormat.RgbaHex => new RgbaHex(),
            ColorFormat.HashRgbaHex => new HashRgbaHex(),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
    public class ArgbObject : ColorConverter
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Color color = default;
            JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                { return color; }
                JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
                ReadOnlySpan<byte> propertyName = reader.ValueSpan;
                reader.Read();
                if (propertyName.SequenceEqual("a"u8) || propertyName.SequenceEqual("alpha"u8))
                    color.A = reader.GetByte();
                else if (propertyName.SequenceEqual("r"u8) || propertyName.SequenceEqual("red"u8))
                    color.R = reader.GetByte();
                else if (propertyName.SequenceEqual("g"u8) || propertyName.SequenceEqual("green"u8))
                    color.G = reader.GetByte();
                else if (propertyName.SequenceEqual("b"u8) || propertyName.SequenceEqual("blue"u8))
                    color.B = reader.GetByte();
                else
                    reader.Skip();
            }
            return color;
        }
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("a", value.A);
            writer.WriteNumber("r", value.R);
            writer.WriteNumber("g", value.G);
            writer.WriteNumber("b", value.B);
            writer.WriteEndObject();
        }
    }
    public class RgbObject : ColorConverter
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Color color = default;
            JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                { return color; }
                JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
                ReadOnlySpan<byte> propertyName = reader.ValueSpan;
                reader.Read();
                if (propertyName.SequenceEqual("r"u8) || propertyName.SequenceEqual("red"u8))
                    color.R = reader.GetByte();
                else if (propertyName.SequenceEqual("g"u8) || propertyName.SequenceEqual("green"u8))
                    color.G = reader.GetByte();
                else if (propertyName.SequenceEqual("b"u8) || propertyName.SequenceEqual("blue"u8))
                    color.B = reader.GetByte();
                else
                    reader.Skip();
            }
            return color;
        }
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("r", value.R);
            writer.WriteNumber("g", value.G);
            writer.WriteNumber("b", value.B);
            writer.WriteEndObject();
        }
    }
    public class ArgbHex : ColorConverter
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (string.IsNullOrEmpty(s)) return default;
            return Color.TryFromArgb(s!, out Color c) ? c : default;
        }
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("aarrggbb"));
        }
    }
    public class HashArgbHex : ColorConverter
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (string.IsNullOrEmpty(s)) return default;
            return Color.TryFromArgb(s!, out Color c) ? c : default;
        }
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("#aarrggbb"));
        }
    }
    public class RgbaHex : ColorConverter
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (string.IsNullOrEmpty(s)) return default;
            return Color.TryFromRgba(s!, out Color c) ? c : default;
        }
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("rrggbbaa"));
        }
    }
    public class HashRgbaHex : ColorConverter
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (string.IsNullOrEmpty(s)) return default;
            return Color.TryFromRgba(s!, out Color c) ? c : default;
        }
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("#rrggbbaa"));
        }
    }
}

//internal class ColorConverter2 : MetadataJsonConverter<Color>
//{
//    private enum ColorFormat
//    {
//        ArgbObject,
//        RgbObject,
//        ArgbHex,
//        HashArgbHex,
//        RgbaHex,
//        HashRgbaHex,
//    }
//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    private static ColorFormat GetColorFormat(LevelType type)
//    {
//        return type switch
//        {
//            LevelType.RhythmDoctor or
//            LevelType.Adofai
//                => ColorFormat.RgbaHex,
//            LevelType.BeatBlock
//                => ColorFormat.RgbObject,
//            _ => throw new JsonException($"Unexpected level type {type} when parsing Color."),
//        };
//    }
//    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
//    {
//        var tokenType = JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.String, JsonTokenType.Null, JsonTokenType.StartObject);
//        switch (GetColorFormat(options.Type))
//        {
//            case ColorFormat.RgbaHex or ColorFormat.HashRgbaHex:
//                string? s = reader.GetString();
//                if (string.IsNullOrEmpty(s)) return default;
//                return Color.TryFromRgba(s!, out Color c) ? c : default;
//            case ColorFormat.ArgbHex or ColorFormat.HashArgbHex:
//                string? s2 = reader.GetString();
//                if (string.IsNullOrEmpty(s2)) return default;
//                return Color.TryFromArgb(s2!, out Color c2) ? c2 : default;
//            case ColorFormat.RgbObject or ColorFormat.ArgbObject:
//                Color color = default;
//                while (reader.Read())
//                {
//                    if (reader.TokenType == JsonTokenType.EndObject)
//                    { return color; }
//                    JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
//                    ReadOnlySpan<byte> propertyName = reader.ValueSpan;
//                    reader.Read();
//                    if (propertyName.SequenceEqual("r"u8) || propertyName.SequenceEqual("red"u8))
//                        color.R = reader.GetByte();
//                    else if (propertyName.SequenceEqual("g"u8) || propertyName.SequenceEqual("green"u8))
//                        color.G = reader.GetByte();
//                    else if (propertyName.SequenceEqual("b"u8) || propertyName.SequenceEqual("blue"u8))
//                        color.B = reader.GetByte();
//                    else if (propertyName.SequenceEqual("a"u8) || propertyName.SequenceEqual("alpha"u8))
//                        color.A = reader.GetByte();
//                    else
//                        reader.Skip();
//                }
//                return color;
//            default:
//                throw new JsonException($"Unexpected token type {tokenType} when parsing Color.");
//        }
//    }

//    public override void Write(Utf8JsonWriter writer, Color value, MetadataJsonSerializerOptions options)
//    {
//        switch (GetColorFormat(options.Type))
//        {
//            case ColorFormat.ArgbObject:
//                writer.WriteStartObject();
//                writer.WriteNumber("r", value.R);
//                writer.WriteNumber("g", value.G);
//                writer.WriteNumber("b", value.B);
//                writer.WriteNumber("a", value.A);
//                writer.WriteEndObject();
//                break;
//            case ColorFormat.RgbObject:
//                writer.WriteStartObject();
//                writer.WriteNumber("r", value.R);
//                writer.WriteNumber("g", value.G);
//                writer.WriteNumber("b", value.B);
//                writer.WriteEndObject();
//                break;
//            case ColorFormat.RgbaHex:
//                writer.WriteStringValue(value.ToString("rrggbbaa"));
//                break;
//            case ColorFormat.HashRgbaHex:
//                writer.WriteStringValue(value.ToString("#rrggbbaa"));
//                break;
//            case ColorFormat.ArgbHex:
//                writer.WriteStringValue(value.ToString("aarrggbb"));
//                break;
//            case ColorFormat.HashArgbHex:
//                writer.WriteStringValue(value.ToString("#aarrggbb"));
//                break;
//        }
//    }
//}
