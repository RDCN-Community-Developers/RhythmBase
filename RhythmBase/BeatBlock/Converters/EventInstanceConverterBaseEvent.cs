using RhythmBase.BeatBlock.Components;
using RhythmBase.BeatBlock.Events;
using RhythmBase.BeatBlock.Settings;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static RhythmBase.BeatBlock.Utils.EventTypeUtils;

namespace RhythmBase.BeatBlock.Converters;

internal class BaseEventConverter : JsonConverter<IBaseEvent>
{
    private ILevelReadSettings<IBaseEvent, EventType, BBBeat> _rs = new LevelReadSettings();
    private ILevelWriteSettings<IBaseEvent, EventType, BBBeat> _ws = new LevelWriteSettings();
    public BaseEventConverter WithReadSettings(ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
    {
        _rs = settings;
        return this;
    }
    public BaseEventConverter WithWriteSettings(ILevelWriteSettings<IBaseEvent, EventType, BBBeat> settings)
    {
        _ws = settings;
        return this;
    }
    public override bool CanConvert(Type typeToConvert)
    {
        return Type.IsAssignableFrom(typeToConvert);
    }
    public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
        ReadOnlySpan<byte> type = default;

        Utf8JsonReader checkpoint = reader;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                if (reader.ValueSpan.SequenceEqual("type"u8))
                {
                    reader.Read();
                    type = reader.ValueSpan;
                    break;
                }
                else
                {
                    reader.Skip();
                }
            }
        }
        reader = checkpoint; IBaseEvent e;
        if (!EnumConverter.TryParse(type, out EventType typeEnum))
            e = ReadForwardEvent(ref reader) ?? new ForwardEvent() { ActualType = type.ToString() ?? "" };
        else
            e = converters[typeEnum].WithReadSettings(_rs).ReadProperties(ref reader, options);
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndObject]);
        return e;
    }
    public static Events.IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        //JsonElement root = doc.RootElement;

        return new ForwardEvent(doc);
    }

    public override void Write(Utf8JsonWriter writer, IBaseEvent value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
internal abstract class EventInstanceConverterBase
{
    protected ILevelReadSettings<IBaseEvent, EventType, BBBeat>? _rs;
    protected ILevelWriteSettings<IBaseEvent, EventType, BBBeat>? _ws;
    internal EventInstanceConverterBase WithReadSettings(ILevelReadSettings<IBaseEvent, EventType, BBBeat> settings)
    {
        _rs = settings;
        return this;
    }
    internal EventInstanceConverterBase WithWriteSettings(ILevelWriteSettings<IBaseEvent, EventType, BBBeat> settings)
    {
        _ws = settings;
        return this;
    }
    public abstract IBaseEvent ReadProperties(ref Utf8JsonReader reader, JsonSerializerOptions options);
    public abstract void WriteProperties(Utf8JsonWriter writer, IBaseEvent value, JsonSerializerOptions options);
}
internal abstract class EventInstanceConverterBaseEvent<TEvent> : EventInstanceConverterBase where TEvent : IBaseEvent, new()
{
    public override sealed IBaseEvent ReadProperties(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        TEvent value = new();
        float time = 0;
        float angle = 0;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                value.Time = time;
                value.Angle = angle;
                return value;
            }
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
            ReadOnlySpan<byte> propertyName = reader.ValueSpan;
            if (propertyName.IsEmpty)
                throw new JsonException("Property name cannot be null.");
            reader.Read();
            if (propertyName.SequenceEqual("time"u8))
                time = reader.GetSingle();
            else if (propertyName.SequenceEqual("angle"u8))
                angle = reader.GetSingle();
            else if (propertyName.SequenceEqual("type"u8))
                continue;
            else if (!Read(ref reader, propertyName, ref value, options))
            {
#if DEBUG
                //if (!(
                //	(value is FloatingText && propertyName.SequenceEqual("times"u8)) ||
                //	(value is FloatingText && propertyName.SequenceEqual("id"u8)) ||
                //	(value is AdvanceText && propertyName.SequenceEqual("id"u8))
                //	))
                //	Console.WriteLine($"The key {Encoding.UTF8.GetString([.. propertyName])} of {value.Type} not found.");
#endif
                value[
                    Encoding.UTF8.GetString(propertyName)
                    ] = JsonElement.ParseValue(ref reader);
            }
        }
        return value;
    }
    public override sealed void WriteProperties(Utf8JsonWriter writer, IBaseEvent value, JsonSerializerOptions options)
    {
        TEvent v = (TEvent)value;
        writer.WriteStartObject();
        Write(writer, ref v, options);
        foreach (KeyValuePair<string, JsonElement> kv in ((BaseEvent)(IBaseEvent)v)._extraData)
        {
            writer.WritePropertyName(kv.Key);
            writer.WriteRawValue(kv.Value.GetRawText());
        }
        writer.WriteEndObject();
    }
    protected virtual bool Read(ref Utf8JsonReader reader, ReadOnlySpan<byte> propertyName, ref TEvent value, JsonSerializerOptions options)
    {
        bool result = true;
        if (propertyName.SequenceEqual("angle"u8))
            value.Angle = reader.GetSingle();
        else if (propertyName.SequenceEqual("time"u8))
            value.Time = reader.GetSingle();
        else if (propertyName.SequenceEqual("variant"u8))
            value.Variant = reader.GetString();
        else if (propertyName.SequenceEqual("order"u8))
            value.Order = reader.GetInt32();
        else
            result = false;
        return result;
    }
    protected virtual void Write(Utf8JsonWriter writer, ref TEvent value, JsonSerializerOptions options)
    {
        writer.WriteNumber("angle"u8, value.Angle);
        writer.WriteNumber("time"u8, value.Time);
        if (value.Variant is not null)
            writer.WriteString("variant"u8, value.Variant);
        if (value.Order is int valueNotNull)
            writer.WriteNumber("order"u8, valueNotNull);
    }
}