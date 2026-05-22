using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Events;
using RhythmBase.Adofai.Settings;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace RhythmBase.Adofai.Converters;

[RDJsonConverterFor(typeof(ADLevel))]
internal class LevelConverter : RDJsonConverter<ADLevel>
{
    private static readonly BaseEventConverter baseEventConverter = new();
    private static readonly SettingsConverter settingsConverter = new();
    internal string? Filepath { get; set; }
    internal ILevelReadSettings<IBaseEvent, EventType, ADBeat> ReadSettings { get; set; } = new LevelReadSettings();
    internal ILevelWriteSettings<IBaseEvent, EventType, ADBeat> WriteSettings { get; set; } = new LevelWriteSettings();

    public override ADLevel? Read(ref Utf8JsonReader reader, Type typeToConvert, RDJsonSerializerOptions options)
    {
        ADLevel level = [];
        bool isTileLoad = false;
        List<BaseTileEvent> tileEventsNotLoad = [];
        reader.Read();
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
        reader.Read();
        while (true)
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
            if (reader.ValueSpan.SequenceEqual("angleData"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                reader.Read();
                while (true)
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;
                    JsonException.ThrowIfNotMatch(reader, [JsonTokenType.Number]);
                    float angle = reader.GetSingle();
                    if (angle == Utils.Utils.MidSpinAngle)
                        level.Add(new Tile(true));
                    else
                        level.Add(new Tile(angle));
                    reader.Read();
                }
                reader.Read();
                isTileLoad = true;
            }
            else if (reader.ValueSpan.SequenceEqual("settings"u8))
            {
                reader.Read();
                level.Settings = settingsConverter.Read(ref reader, typeof(Components.Settings), options.JsonSerializerOptions) ?? new();
                if (level.Settings.Version < MinimumSupportedVersionAdofai)
#if DEBUG
                    Console.WriteLine($"Current version {level.Settings.Version} is too low.");
#else
					throw new VersionTooLowException(MinimumSupportedVersionAdofai);
#endif
            }
            else if (reader.ValueSpan.SequenceEqual("actions"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                reader.Read();
                while (true)
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;
                    IBaseEvent? e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
                    if (e == null)
                        continue;
                    if (e is BaseTileEvent tileE)
                    {
                        if (isTileLoad)
                            level[tileE._floor].Add(tileE);
                        else
                            tileEventsNotLoad.Add(tileE);
                    }
                }
                reader.Read();
            }
            else if (reader.ValueSpan.SequenceEqual("decorations"u8))
            {
                reader.Read();
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                reader.Read();
                while (true)
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;
                    IBaseEvent? e = baseEventConverter.Read(ref reader, typeof(BaseEvent), options) as BaseEvent;
                    if (e != null)
                        level.Decorations.Add(e);
                }
                reader.Read();
            }
            else
            {
                reader.Skip();
                reader.Read();
            }
        }
        reader.Read();
        return level;
    }

    public override void Write(Utf8JsonWriter writer, ADLevel value, RDJsonSerializerOptions options)
    {
        using MemoryStream stream = new();
        RDJsonSerializerOptions localOptions = new()
        {
            Type = LevelType.Adofai,
            JsonSerializerOptions = new JsonSerializerOptions(options.JsonSerializerOptions)
            { WriteIndented = false }
        };
        using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, localOptions);
        ReadOnlySpan<byte> sl;
        writer.WriteStartObject();
        writer.WritePropertyName("angleData");
        using Utf8JsonWriter noIndentWriter = new(stream, new JsonWriterOptions { Indented = false });
        noIndentWriter.WriteStartArray();
        foreach (Tile tile in value)
        {
            if (tile.IsMidSpin)
                noIndentWriter.WriteNumberValue(Utils.Utils.MidSpinAngle);
            else
                noIndentWriter.WriteNumberValue(tile.Angle);
        }
        noIndentWriter.WriteEndArray();
        noIndentWriter.Flush();
        sl = stream.GetBuffer().AsSpan(0, (int)stream.Position);
        writer.WriteRawValue(sl);
        noIndentWriter.Reset();
        writer.WritePropertyName("settings");
        settingsConverter.Write(writer, value.Settings, options.JsonSerializerOptions);
        writer.WriteStartArray("actions");
        noIndentScope.WriteNoIndentTo(false, writer, value.SelectMany(i => i.Cast<IBaseEvent>()), baseEventConverter.Write);
        writer.WriteEndArray();
        writer.WriteStartArray("decorations");
        noIndentScope.WriteNoIndentTo(false, writer, value.Decorations, baseEventConverter.Write);
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
