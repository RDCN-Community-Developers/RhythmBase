using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

public abstract class MetadataJsonConverter<T> : JsonConverter<T>
{
    public MetadataJsonSerializerOptions? JsonSerializerOptions { get; internal set; }
    public MetadataJsonConverter<T> WithOptions(JsonSerializerOptions options)
    {
        this.JsonSerializerOptions ??= new MetadataJsonSerializerOptions { Type = LevelType.Unknown, JsonSerializerOptions = options };
        return this;
    }
    public MetadataJsonConverter<T> WithOptions(MetadataJsonSerializerOptions options)
    {
        this.JsonSerializerOptions = options;
        return this;
    }
    public abstract T? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options);
    public abstract void Write(Utf8JsonWriter writer, T value, MetadataJsonSerializerOptions options);
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Read(ref reader, typeToConvert, this.JsonSerializerOptions ?? new MetadataJsonSerializerOptions { Type = LevelType.Unknown, JsonSerializerOptions = options });
    }
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        Write(writer, value, this.JsonSerializerOptions ?? new MetadataJsonSerializerOptions { Type = LevelType.Unknown, JsonSerializerOptions = options });
    }
}
