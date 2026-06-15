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
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
            if (reader.ValueTextEquals("character"u8) && reader.Read())
            {
                string character = reader.GetString() ?? "";
                if (character.StartsWith("custom:"))
                    result.Character = character[7..];
                else if (EnumConverter.TryParse(character, out Characters rdc))
                    result.Character = rdc;
            }
            else if (reader.ValueTextEquals("cpuMarker"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out Characters value0))
                result.CpuMarker = value0;
            else if (reader.ValueTextEquals("rowType"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out RowType value1))
                result.RowType = value1;
            else if (reader.ValueTextEquals("rooms"u8) && reader.Read())
                result.Room = TypeConverterRegistry.Read<SingleRoom>(ref reader, options);
            else if (reader.ValueTextEquals("hideAtStart"u8) && reader.Read())
                result.HideAtStart = reader.GetBoolean();
            else if (reader.ValueTextEquals("player"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out PlayerType value2))
                result.Player = value2;
            else if (reader.ValueTextEquals("muteBeats"u8) && reader.Read())
                result.MuteBeats = reader.GetBoolean();
            else if (reader.ValueTextEquals("rowToMimic"u8) && reader.Read())
                result.RowToMimic = reader.GetSByte();
            else if (reader.ValueTextEquals("pulseSound"u8) && reader.Read())
                result.Sound.Filename = reader.GetString() ?? "";
            else if (reader.ValueTextEquals("pulseSoundVolume"u8) && reader.Read())
                result.Sound.Volume = reader.GetInt32();
            else if (reader.ValueTextEquals("pulseSoundPitch"u8) && reader.Read())
                result.Sound.Pitch = reader.GetInt32();
            else if (reader.ValueTextEquals("pulseSoundPan"u8) && reader.Read())
                result.Sound.Pan = reader.GetInt32();
            else if (reader.ValueTextEquals("pulseSoundOffset"u8) && reader.Read())
                result.Sound.Offset = TimeSpan.FromMilliseconds(reader.GetDouble());
            else
                reader.Skip();
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
        TypeConverterRegistry.Write(writer, value.Room, options);

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
