using RhythmBase.BeatBlock.Components;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Converters;

[JsonConverterFor(typeof(Level))]
internal class ManifestConverter : MetadataJsonConverter<Level>
{
    public override Level? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
    {
        reader.Read();
        Level level = new();
        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
            ReadOnlySpan<byte> propertyName = reader.ValueSpan;
            reader.Read();
            if (propertyName.SequenceEqual("defaultVariant"u8))
                level.DefaultVariant = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
            else if (propertyName.SequenceEqual("metadata"u8))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;
                    JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
                    ReadOnlySpan<byte> metadataPropertyName = reader.ValueSpan;
                    reader.Read();
                    if (metadataPropertyName.SequenceEqual("artist"u8))
                        level.Metadata.Artist = reader.GetString() ?? "";
                    else if (metadataPropertyName.SequenceEqual("artistLink"u8))
                        level.Metadata.ArtistLink = reader.GetString() ?? "";
                    else if (metadataPropertyName.SequenceEqual("bg"u8))
                        level.Metadata.IsBackgroundEnabled = reader.GetBoolean();
                    else if (metadataPropertyName.SequenceEqual("charter"u8))
                        level.Metadata.Charter = reader.GetString() ?? "";
                    else if (metadataPropertyName.SequenceEqual("description"u8))
                        level.Metadata.Description = reader.GetString() ?? "";
                    else if (metadataPropertyName.SequenceEqual("difficulty"u8))
                        level.Metadata.Difficulty = reader.GetInt32();
                    else if (metadataPropertyName.SequenceEqual("startLoop"u8))
                        level.Metadata.StartLoop = reader.GetInt32();
                    else if (metadataPropertyName.SequenceEqual("endLoop"u8))
                        level.Metadata.EndLoop = reader.GetInt32();
                    else if (metadataPropertyName.SequenceEqual("lightWarning"u8))
                        level.Metadata.LightWarning = reader.GetBoolean();
                    else if (metadataPropertyName.SequenceEqual("loopPointsEnable"u8))
                        level.Metadata.LoopPointsEnable = reader.GetBoolean();
                    else if (metadataPropertyName.SequenceEqual("lyricsWarning"u8))
                        level.Metadata.LyricsWarning = reader.GetBoolean();
                    else if (metadataPropertyName.SequenceEqual("songName"u8))
                        level.Metadata.SongName = reader.GetString() ?? "";
                    else if (metadataPropertyName.SequenceEqual("source"u8))
                        level.Metadata.Source = reader.GetString() ?? "";
                    else if (metadataPropertyName.SequenceEqual("bgData"u8))
                    {
                        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndObject)
                                break;
                            JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
                            ReadOnlySpan<byte> bgDataPropertyName = reader.ValueSpan;
                            reader.Read();
                            if (bgDataPropertyName.SequenceEqual("redChannel"u8))
                                level.Metadata.BackgroundData.RedChannel = ConverterHub.Read<Color>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("greenChannel"u8))
                                level.Metadata.BackgroundData.GreenChannel = ConverterHub.Read<Color>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("blueChannel"u8))
                                level.Metadata.BackgroundData.BlueChannel = ConverterHub.Read<Color>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("yellowChannel"u8))
                                level.Metadata.BackgroundData.YellowChannel = ConverterHub.Read<Color>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("cyanChannel"u8))
                                level.Metadata.BackgroundData.CyanChannel = ConverterHub.Read<Color>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("magentaChannel"u8))
                                level.Metadata.BackgroundData.MagentaChannel = ConverterHub.Read<Color>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("hideCranky"u8))
                                level.Metadata.BackgroundData.HideCranky = reader.GetBoolean();
                            else if (bgDataPropertyName.SequenceEqual("image"u8))
                                level.Metadata.BackgroundData.Image = ConverterHub.Read<FileReference>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("resultsImage"u8))
                                level.Metadata.BackgroundData.ResultsImage = ConverterHub.Read<FileReference>(ref reader, options);
                            else
                                reader.Skip();
                        }
                    }
                    else
                        reader.Skip();
                }
            }
            else if (propertyName.SequenceEqual("properties"u8))
            {
                JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;
                    JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
                    ReadOnlySpan<byte> propertiesPropertyName = reader.ValueSpan;
                    reader.Read();
                    if (propertiesPropertyName.SequenceEqual("formatVersion"u8))
                        level.Properties.FormatVersion = reader.GetInt32();
                    else if (propertiesPropertyName.SequenceEqual("offset"u8))
                        level.Properties.Offset = reader.GetInt32();
                    else if (propertiesPropertyName.SequenceEqual("startingBeat"u8))
                        level.Properties.StartingBeat = reader.GetInt32();
                    else if (propertiesPropertyName.SequenceEqual("loadBeat"u8))
                        level.Properties.LoadBeat = reader.GetInt32();
                    else if (propertiesPropertyName.SequenceEqual("formatVersion"u8))
                        level.Properties.FormatVersion = reader.GetInt32();
                    else
                        reader.Skip();
                }
            }
            else if (propertyName.SequenceEqual("variants"u8))
            {
                JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
                List<Chart> variants = [];
                List<int> slotIndex = [];
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;
                    JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
                    Chart variant = [];
                    int index = 0;
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break;
                        JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
                        ReadOnlySpan<byte> variantPropertyName = reader.ValueSpan;
                        reader.Read();
                        if (variantPropertyName.SequenceEqual("charter"u8))
                            variant.Charter = reader.GetString() ?? "";
                        else if (variantPropertyName.SequenceEqual("difficulty"u8))
                            variant.Difficulty = reader.GetInt32();
                        else if (variantPropertyName.SequenceEqual("display"u8))
                            variant.Display = reader.GetString() ?? "";
                        else if (variantPropertyName.SequenceEqual("extra"u8))
                            variant.Extra = reader.GetBoolean();
                        else if (variantPropertyName.SequenceEqual("hidden"u8))
                            variant.Hidden = reader.GetBoolean();
                        else if (variantPropertyName.SequenceEqual("name"u8))
                            variant.Name = reader.GetString() ?? "";
                        else if (variantPropertyName.SequenceEqual("slot"u8))
                            index = reader.GetInt32();
                        else if (variantPropertyName.SequenceEqual("levelFile"u8))
                            variant.LevelFile = reader.GetString();
                        else
                            reader.Skip();
                    }
                    variants.Add(variant);
                    slotIndex.Add(index);
                }
                Chart[] variants1 = [.. variants];
                Array.Sort(slotIndex.ToArray(), variants1);
                foreach (var variant in variants1)
                {
                    level.Variants[variant.Name] = variant;
                }
            }
            else
            {
                reader.Skip();
            }
        }
        return level;
    }

    public override void Write(Utf8JsonWriter writer, Level value, MetadataJsonSerializerOptions options)
    {
        MetadataJsonSerializerOptions localOptions = new() {
            JsonSerializerOptions = options.JsonSerializerOptions,
            WriteAligned = false 
        };
        using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, localOptions);
        writer.WriteStartObject();
        writer.WriteString("defaultVariant"u8, value.DefaultVariant);
        #region metadata
        writer.WriteStartObject("metadata"u8);
        writer.WriteString("songName"u8, value.Metadata.SongName);
        writer.WriteString("artist"u8, value.Metadata.Artist);
        writer.WriteString("artistLink"u8, value.Metadata.ArtistLink);
        writer.WriteNumber("bpm"u8, value.Metadata.Bpm);
        writer.WriteString("description"u8, value.Metadata.Description);
        writer.WriteString("charter"u8, value.Metadata.Charter);
        writer.WriteNumber("difficulty"u8, value.Metadata.Difficulty);
        writer.WriteBoolean("lightWarning"u8, value.Metadata.LightWarning);
        writer.WriteBoolean("lyricsWarning"u8, value.Metadata.LyricsWarning);
        writer.WriteBoolean("loopPointsEnable"u8, value.Metadata.LoopPointsEnable);
        writer.WriteString("source"u8, value.Metadata.Source);
        writer.WriteNumber("startLoop"u8, value.Metadata.StartLoop);
        writer.WriteNumber("endLoop"u8, value.Metadata.EndLoop);
        writer.WriteBoolean("bg"u8, value.Metadata.IsBackgroundEnabled);
        writer.WriteEndObject();
        #region bgdata
        writer.WriteStartObject("bgData");
        writer.WritePropertyName("redChannel");
        noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.RedChannel, ConverterHub.Write);
        writer.WritePropertyName("greenChannel");
        noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.GreenChannel, ConverterHub.Write);
        writer.WritePropertyName("blueChannel");
        noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.BlueChannel, ConverterHub.Write);
        writer.WritePropertyName("yellowChannel");
        noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.YellowChannel, ConverterHub.Write);
        writer.WritePropertyName("cyanChannel");
        noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.CyanChannel, ConverterHub.Write);
        writer.WritePropertyName("magentaChannel");
        noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.MagentaChannel, ConverterHub.Write);
        writer.WriteBoolean("hideCranky"u8, value.Metadata.BackgroundData.HideCranky);
        writer.WritePropertyName("image");
        ConverterHub.Write(writer, value.Metadata.BackgroundData.Image, options);
        writer.WritePropertyName("resultsImage");
        ConverterHub.Write(writer, value.Metadata.BackgroundData.ResultsImage, options);
        writer.WriteEndObject();
        #endregion
        #endregion
        #region properties
        writer.WriteStartObject("properties");
        writer.WriteNumber("formatVersion"u8, value.Properties.FormatVersion);
        writer.WriteEndObject();
        #endregion
        #region variants
        writer.WriteStartArray("variants");
        noIndentScope.WriteNoIndentArrayTo(options, writer, value.Variants, (w, v, o) => {
            w.WriteStartObject();
            w.WriteString("name"u8, v.Name);
            w.WriteString("charter"u8, v.Charter);
            w.WriteNumber("difficulty"u8, v.Difficulty);
            w.WriteString("display"u8, v.Display);
            w.WriteBoolean("extra"u8, v.Extra);
            w.WriteBoolean("hidden"u8, v.Hidden);
            w.WriteEndObject();
        });
        writer.WriteEndArray();
        #endregion
        writer.WriteEndObject();
    }
}
