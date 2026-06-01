using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Row))]
internal class RowConverter : MetadataJsonConverter<Row>
{
    public override Row? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
    {
        Row result = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
            ReadOnlySpan<byte> propertyName = reader.ValueSpan;
            reader.Read();
            if (propertyName.SequenceEqual("character"u8))
            {
                string character = reader.GetString() ?? "";
                if (character.StartsWith("custom:"))
                    result.Character = character[7..];
                else if (EnumConverter.TryParse(character, out Characters rdc))
                    result.Character = rdc;
            }
            else if (propertyName.SequenceEqual("cpuMarker"u8) && EnumConverter.TryParse(reader.ValueSpan, out Characters value0))
                result.CpuMarker = value0;
            else if (propertyName.SequenceEqual("rowType"u8) && EnumConverter.TryParse(reader.ValueSpan, out RowType value1))
                result.RowType = value1;
            else if (propertyName.SequenceEqual("rooms"u8))
                result.Room = ConverterHub.Read<SingleRoom>(ref reader, options);
            else if (propertyName.SequenceEqual("hideAtStart"u8))
                result.HideAtStart = reader.GetBoolean();
            else if (propertyName.SequenceEqual("player"u8) && EnumConverter.TryParse(reader.ValueSpan, out PlayerType value2))
                result.Player = value2;
            else if (propertyName.SequenceEqual("muteBeats"u8))
                result.MuteBeats = reader.GetBoolean();
            else if (propertyName.SequenceEqual("rowToMimic"u8))
                result.RowToMimic = reader.GetSByte();
            else if (propertyName.SequenceEqual("pulseSound"u8))
                result.Sound.Filename = reader.GetString() ?? "";
            else if (propertyName.SequenceEqual("pulseSoundVolume"u8))
                result.Sound.Volume = reader.GetInt32();
            else if (propertyName.SequenceEqual("pulseSoundPitch"u8))
                result.Sound.Pitch = reader.GetInt32();
            else if (propertyName.SequenceEqual("pulseSoundPan"u8))
                result.Sound.Pan = reader.GetInt32();
            else if (propertyName.SequenceEqual("pulseSoundOffset"u8))
                result.Sound.Offset = TimeSpan.FromMilliseconds(reader.GetDouble());
        }
        return result;
    }

    public override void Write(Utf8JsonWriter writer, Row value, MetadataJsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("character"u8);
        if (value.Character.IsCustom)
            writer.WriteStringValue($"custom:{value.Character}");
        else
            writer.WriteStringValue(value.Character.Character.ToEnumString());
        if (value.Player == PlayerType.CPU)
            writer.WriteString("cpuMarker"u8, value.CpuMarker.ToEnumString());
        writer.WriteString("rowType"u8, value.RowType.ToEnumString());
        writer.WriteNumber("row"u8, value.Index);

        writer.WritePropertyName("rooms"u8);
        ConverterHub.Write(writer, value.Room, options);

        if (value.HideAtStart)
            writer.WriteBoolean("hideAtStart"u8, value.HideAtStart);
        writer.WriteString("player"u8, value.Player.ToEnumString());
        if (value.MuteBeats)
            writer.WriteBoolean("muteBeats"u8, value.MuteBeats);

        if (value.RowToMimic >= 0)
            writer.WriteNumber("rowToMimic"u8, value.RowToMimic);

        writer.WriteString("pulseSound"u8, value.Sound.Filename);

        if (value.Sound.Volume != 100)
            writer.WriteNumber("pulseSoundVolume"u8, value.Sound.Volume);

        if (value.Sound.Pitch != 100)
            writer.WriteNumber("pulseSoundPitch"u8, value.Sound.Pitch);

        if (value.Sound.Pan != 0)
            writer.WriteNumber("pulseSoundPan"u8, value.Sound.Pan);

        if (value.Sound.Offset != TimeSpan.Zero)
            writer.WriteNumber("pulseSoundOffset"u8, (int)value.Sound.Offset.TotalMilliseconds);

        writer.WriteEndObject();
    }
}
