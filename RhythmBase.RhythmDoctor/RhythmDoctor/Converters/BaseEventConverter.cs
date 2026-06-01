using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Events;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

internal class BaseEventConverter : MetadataJsonConverter<IBaseEvent>
{
    private LevelReadSettings _rs = new LevelReadSettings();
    private LevelWriteSettings _ws = new LevelWriteSettings();
    public BaseEventConverter WithReadSettings(LevelReadSettings settings)
    {
        _rs = settings;
        return this;
    }
    public BaseEventConverter WithWriteSettings(LevelWriteSettings settings)
    {
        _ws = settings;
        return this;
    }
    public override bool CanConvert(Type typeToConvert)
    {
        return Type.IsAssignableFrom(typeToConvert);
    }
    public override IBaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
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
            e = ReadForwardEvent(ref reader) ?? (new ForwardEvent() { ActualType = type.ToString() ?? "" });
        else
            e = ConverterMap.GetConverter(typeEnum).WithReadSettings(_rs).ReadProperties(ref reader, options);
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.EndObject]);
        reader.Read();
        return e;
    }
    public override void Write(Utf8JsonWriter writer, IBaseEvent value, MetadataJsonSerializerOptions options)
    {
        if (value is Events.IForwardEvent ce)
        {
            WriteForwardEvent(writer, ce);
            return;
        }
        else
        {
            ConverterMap.GetConverter(value.Type).WithWriteSettings(_ws).WriteProperties(writer, value, options);
        }
    }
    public static Events.IForwardEvent? ReadForwardEvent(ref Utf8JsonReader reader)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        // 判断属性
        bool hasRow = false, hasTarget = false;
        foreach (JsonProperty prop in root.EnumerateObject())
        {
            if (prop.NameEquals("row"))
                hasRow = true;
            else if (prop.NameEquals("target"))
                hasTarget = true;
        }
        if (hasRow) return new ForwardRowEvent(doc);
        else return hasTarget ? new ForwardDecorationEvent(doc) : new ForwardEvent(doc);
    }

    public static void WriteForwardEvent(Utf8JsonWriter writer, Events.IForwardEvent value)
    {
        (int bar, float beat) = value.TickTime;
        writer.WriteStartObject();
        if (!string.IsNullOrEmpty(value.ActualType))
            writer.WriteString("type", value.ActualType);
        writer.WriteNumber("bar", bar);
        writer.WriteNumber("beat", beat);
        if (value is ForwardRowEvent rowEvent)
            writer.WriteNumber("row", rowEvent.Index);
        else if (value is ForwardDecorationEvent decorationEvent)
            writer.WriteString("target", decorationEvent.Target);
        if (!string.IsNullOrEmpty(value.Tag))
            writer.WriteString("tag", value.Tag);
        if (value.RunTag)
            writer.WriteBoolean("runTag", value.RunTag);
        if (!value.Active)
            writer.WriteBoolean("active", value.Active);
        if (value.Condition.HasValue)
            writer.WriteString("if", value.Condition.Serialize());
        if (value.Y != 0)
            writer.WriteNumber("y", value.Y);

        foreach (KeyValuePair<string, JsonElement> kv in ((BaseEvent)value)._extraData)
        {
            writer.WritePropertyName(kv.Key);
            kv.Value.WriteTo(writer);
        }
        writer.WriteEndObject();
    }
}