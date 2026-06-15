using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

/// <summary>
/// Abstract base class for JSON converters that handle <see cref="Color"/> values in various formats.
/// </summary>
public abstract class ColorConverter : JsonConverter<Color>
{
    /// <summary>
    /// Specifies the supported color serialization formats.
    /// </summary>
    public enum ColorFormat
    {
        /// <summary>JSON object with <c>a</c>, <c>r</c>, <c>g</c>, <c>b</c> properties.</summary>
        ArgbObject,
        /// <summary>JSON object with <c>r</c>, <c>g</c>, <c>b</c> properties.</summary>
        RgbObject,
        /// <summary>Hex string in <c>aarrggbb</c> format.</summary>
        ArgbHex,
        /// <summary>Hex string in <c>#aarrggbb</c> format.</summary>
        HashArgbHex,
        /// <summary>Hex string in <c>rrggbbaa</c> format.</summary>
        RgbaHex,
        /// <summary>Hex string in <c>#rrggbbaa</c> format.</summary>
        HashRgbaHex,
    }
    /// <summary>
    /// Creates a <see cref="ColorConverter"/> instance for the specified format.
    /// </summary>
    /// <param name="format">The desired color format.</param>
    /// <returns>A new converter instance for the specified format.</returns>
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
    /// <summary>Converter for <see cref="Color"/> as a JSON object with ARGB components.</summary>
    public class ArgbObject : ColorConverter
    {
        /// <inheritdoc/>
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Color color = default;
            JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                { return color; }
                JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
                if (reader.ValueTextEquals("a"u8) || reader.ValueTextEquals("alpha"u8))
                {
                    reader.Read();
                    color.A = reader.GetByte();
                }
                else if (reader.ValueTextEquals("r"u8) || reader.ValueTextEquals("red"u8))
                {
                    reader.Read();
                    color.R = reader.GetByte();
                }
                else if (reader.ValueTextEquals("g"u8) || reader.ValueTextEquals("green"u8))
                {
                    reader.Read();
                    color.G = reader.GetByte();
                }
                else if (reader.ValueTextEquals("b"u8) || reader.ValueTextEquals("blue"u8))
                {
                    reader.Read();
                    color.B = reader.GetByte();
                }
                else
                    reader.Skip();
            }
            return color;
        }
        /// <inheritdoc/>
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
    /// <summary>Converter for <see cref="Color"/> as a JSON object with RGB components.</summary>
    public class RgbObject : ColorConverter
    {
        /// <inheritdoc/>
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Color color = default;
            JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                { return color; }
                JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
                if (reader.ValueTextEquals("r"u8) || reader.ValueTextEquals("red"u8))
                {
                    reader.Read();
                    color.R = reader.GetByte();
                }
                else if (reader.ValueTextEquals("g"u8) || reader.ValueTextEquals("green"u8))
                {
                    reader.Read();
                    color.G = reader.GetByte();
                }
                else if (reader.ValueTextEquals("b"u8) || reader.ValueTextEquals("blue"u8))
                {
                    reader.Read();
                    color.B = reader.GetByte();
                }
                else
                    reader.Skip();
            }
            return color;
        }
        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("r", value.R);
            writer.WriteNumber("g", value.G);
            writer.WriteNumber("b", value.B);
            writer.WriteEndObject();
        }
    }
    /// <summary>Converter for <see cref="Color"/> as a hex string in <c>aarrggbb</c> format.</summary>
    public class ArgbHex : ColorConverter
    {
        /// <inheritdoc/>
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (string.IsNullOrEmpty(s)) return default;
            return Color.TryFromArgb(s!, out Color c) ? c : default;
        }
        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("aarrggbb"));
        }
    }
    /// <summary>Converter for <see cref="Color"/> as a hex string in <c>#aarrggbb</c> format.</summary>
    public class HashArgbHex : ColorConverter
    {
        /// <inheritdoc/>
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (string.IsNullOrEmpty(s)) return default;
            return Color.TryFromArgb(s!, out Color c) ? c : default;
        }
        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("#aarrggbb"));
        }
    }
    /// <summary>Converter for <see cref="Color"/> as a hex string in <c>rrggbbaa</c> format.</summary>
    public class RgbaHex : ColorConverter
    {
        /// <inheritdoc/>
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (string.IsNullOrEmpty(s)) return default;
            return Color.TryFromRgba(s!, out Color c) ? c : default;
        }
        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("rrggbbaa"));
        }
    }
    /// <summary>Converter for <see cref="Color"/> as a hex string in <c>#rrggbbaa</c> format.</summary>
    public class HashRgbaHex : ColorConverter
    {
        /// <inheritdoc/>
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if (string.IsNullOrEmpty(s)) return default;
            return Color.TryFromRgba(s!, out Color c) ? c : default;
        }
        /// <inheritdoc/>
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
//    /// <inheritdoc/>    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
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

//    /// <inheritdoc/>    public override void Write(Utf8JsonWriter writer, Color value, MetadataJsonSerializerOptions options)
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
