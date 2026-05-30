using RhythmBase.Global.Components.RichText;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

public class ListConverter<T> : MetadataJsonConverter<List<T>>
{
    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
    {
        List<T> result = new();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;
            else
                result.Add(ConverterHub.Read<T>(ref reader, options));
        }
        return result;
    }
    public override void Write(Utf8JsonWriter writer, List<T> value, MetadataJsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (T item in value)
            ConverterHub.Write(writer, item, options);
        writer.WriteEndArray();
    }
}
//internal static partial class ConverterHub
//{
//    private static class ConverterCache<T>
//    {
//        public static JsonConverter? Converter;
//    }
//    static ConverterHub()
//    {
//        //InitializeConverters();
//        //ConverterCache<List<string>>.Converter = new ListConverter<string>();
//        //ConverterCache<List<FileReference>>.Converter = new ListConverter<FileReference>();
//        //ConverterCache<RichLine<RichStringStyle>>.Converter = new RichTextConverter<RichStringStyle>();
//    }

//    public static T Read<T>(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
//    {
//        if (ConverterCache<T>.Converter is MetadataJsonConverter<T> rdconverter)
//            return rdconverter.Read(ref reader, typeof(T), options) ?? default!;
//        if (ConverterCache<T>.Converter is JsonConverter<T> converter)
//            return converter.Read(ref reader, typeof(T), options.JsonSerializerOptions) ?? default!;
//        else
//#if DEBUG
//            throw new NotImplementedException($"No converter found for type {typeof(T)}");
//#else
//            return default!;
//#endif
//    }
//    public static void Write<T>(Utf8JsonWriter writer, T value, MetadataJsonSerializerOptions options)
//    {
//        if (ConverterCache<T>.Converter is MetadataJsonConverter<T> rdconverter)
//            rdconverter.Write(writer, value, options);
//        else if (ConverterCache<T>.Converter is JsonConverter<T> converter)
//            converter.Write(writer, value, options.JsonSerializerOptions);
//        else
//#if DEBUG
//            throw new NotImplementedException($"No converter found for type {typeof(T)}");
//#else
//            writer.WriteNullValue();
//#endif
//    }
//}
