using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

internal abstract class RDJsonConverter<T> : JsonConverter<T>
{
    public RDJsonSerializerOptions? JsonSerializerOptions { get; internal set; }
    public RDJsonConverter<T> WithOptions(JsonSerializerOptions options)
    {
        this.JsonSerializerOptions ??= new RDJsonSerializerOptions { Type = LevelType.Unknown, JsonSerializerOptions = options };
        return this;
    }
    public RDJsonConverter<T> WithOptions(RDJsonSerializerOptions options)
    {
        this.JsonSerializerOptions = options;
        return this;
    }
    public abstract T? Read(ref Utf8JsonReader reader, Type typeToConvert, RDJsonSerializerOptions options);
    public abstract void Write(Utf8JsonWriter writer, T value, RDJsonSerializerOptions options);
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
        return Read(ref reader, typeToConvert, this.JsonSerializerOptions ?? new RDJsonSerializerOptions { Type = LevelType.Unknown, JsonSerializerOptions = options });
    }
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
        Write(writer, value, this.JsonSerializerOptions ?? new RDJsonSerializerOptions { Type = LevelType.Unknown, JsonSerializerOptions = options });
    }
}
