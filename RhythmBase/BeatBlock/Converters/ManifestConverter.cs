using RhythmBase.BeatBlock.Components;
using RhythmBase.RhythmDoctor.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.BeatBlock.Converters;

[RDJsonConverterFor(typeof(BBLevel))]
internal class ManifestConverter : JsonConverter<BBLevel>
{
    public override BBLevel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.Read();
        BBLevel level = [];
        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
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
                    JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
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
                        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndObject)
                                break;
                            JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
                            ReadOnlySpan<byte> bgDataPropertyName = reader.ValueSpan;
                            reader.Read();
                            if (bgDataPropertyName.SequenceEqual("redChannel"u8))
                                level.Metadata.BackgroundData.RedChannel = ConverterHub.Read<RDColor>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("greenChannel"u8))
                                level.Metadata.BackgroundData.GreenChannel = ConverterHub.Read<RDColor>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("blueChannel"u8))
                                level.Metadata.BackgroundData.BlueChannel = ConverterHub.Read<RDColor>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("yellowChannel"u8))
                                level.Metadata.BackgroundData.YellowChannel = ConverterHub.Read<RDColor>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("cyanChannel"u8))
                                level.Metadata.BackgroundData.CyanChannel = ConverterHub.Read<RDColor>(ref reader, options);
                            else if (bgDataPropertyName.SequenceEqual("magentaChannel"u8))
                                level.Metadata.BackgroundData.MagentaChannel = ConverterHub.Read<RDColor>(ref reader, options);
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
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;
                    JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
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
                JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartArray]);
                List<Variant> variants = [];
                List<int> slotIndex = [];
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;
                    JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
                    Variant variant = new();
                    int index = 0;
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break;
                        JsonException.ThrowIfNotMatch(reader, [JsonTokenType.PropertyName]);
                        ReadOnlySpan<byte> variantPropertyName = reader.ValueSpan;
                        string variantPropertyNameString = Encoding.UTF8.GetString(variantPropertyName);
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
                        else
                            reader.Skip();
                    }
                    variants.Add(variant);
                    slotIndex.Add(index);
                }
                Variant[] variants1 = variants.ToArray();
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

    public override void Write(Utf8JsonWriter writer, BBLevel value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("defaultVariant", value.DefaultVariant);
        writer.WriteStartObject("metadata");
        writer.WriteString("artist", value.Metadata.Artist);
        writer.WriteString("artistLink", value.Metadata.ArtistLink);
        writer.WriteBoolean("bg", value.Metadata.IsBackgroundEnabled);
        writer.WriteStartObject("bgData");
        writer.WritePropertyName("redChannel");
        ConverterHub.Write(writer, value.Metadata.BackgroundData.RedChannel, options);
    }
}
