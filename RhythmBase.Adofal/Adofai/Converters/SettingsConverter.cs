using RhythmBase.Adofai.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Adofai.Converters;

[JsonConverterFor(typeof( Components.Settings))]
internal class SettingsConverter : JsonConverter< Components.Settings>
{
    private static readonly RDPointNIConverter pointsConverter = new();
    public override Components.Settings? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"Expected StartObject token, but got {reader.TokenType}.");
        Components.Settings settings = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"Expected PropertyName token, but got {reader.TokenType}.");
            ReadOnlySpan<byte> propertyName = reader.ValueSpan;
            reader.Read();
            if (propertyName.SequenceEqual("version"u8))
                settings.Version = reader.GetInt32();
            else if (propertyName.SequenceEqual("artist"u8))
                settings.Artist = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("specialArtistType"u8) && EnumConverter.TryParse(reader.ValueSpan, out SpecialArtistTypes specialArtistTypes))
                settings.SpecialArtistType = specialArtistTypes;
            else if (propertyName.SequenceEqual("artistPermission"u8))
                settings.ArtistPermission = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("song"u8))
                settings.Song = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("author"u8))
                settings.Author = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("separateCountdownTime"u8))
                settings.SeparateCountdownTime = reader.GetBoolean();
            else if (propertyName.SequenceEqual("previewImage"u8))
                settings.PreviewImage = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("previewIcon"u8))
                settings.PreviewIcon = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("previewIconColor"u8) && Color.TryFromRgba(reader.ValueSpan, out Color color))
                settings.PreviewIconColor = color;
            else if (propertyName.SequenceEqual("previewSongStart"u8))
                settings.PreviewSongStart = reader.GetInt32();
            else if (propertyName.SequenceEqual("previewSongDuration"u8))
                settings.PreviewSongDuration = reader.GetInt32();
            else if (propertyName.SequenceEqual("seizureWarning"u8))
                settings.SeizureWarning = reader.GetBoolean();
            else if (propertyName.SequenceEqual("levelDesc"u8))
                settings.LevelDesc = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("levelTags"u8))
                settings.LevelTags = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("artistLinks"u8))
                settings.ArtistLinks = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("speedTrialAim"u8))
                settings.SpeedTrialAim = reader.GetSingle();
            else if (propertyName.SequenceEqual("difficulty"u8))
                settings.Difficulty = reader.GetInt32();
            else if (propertyName.SequenceEqual("requiredMods"u8))
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException($"Expected StartArray token, but got {reader.TokenType}.");
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType != JsonTokenType.String)
                        throw new JsonException($"Expected String token, but got {reader.TokenType}.");
                    settings.RequiredMods.Add(reader.GetString() ?? string.Empty);
                }
            }
            else if (propertyName.SequenceEqual("songFilename"u8))
                settings.SongFilename = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("bpm"u8))
                settings.Bpm = reader.GetSingle();
            else if (propertyName.SequenceEqual("volume"u8))
                settings.Volume = reader.GetInt32();
            else if (propertyName.SequenceEqual("offset"u8))
                settings.Offset = reader.GetInt32();
            else if (propertyName.SequenceEqual("pitch"u8))
                settings.Pitch = reader.GetInt32();
            else if (propertyName.SequenceEqual("hitsound"u8))
                settings.Hitsound = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("hitsoundVolume"u8))
                settings.HitsoundVolume = reader.GetInt32();
            else if (propertyName.SequenceEqual("countdownTicks"u8))
                settings.CountdownTicks = reader.GetInt32();
            else if (propertyName.SequenceEqual("tileShape"u8) && EnumConverter.TryParse(reader.ValueSpan, out TileShape tileShape))
                settings.TileShape = tileShape;
            else if (propertyName.SequenceEqual("trackColorType"u8) && EnumConverter.TryParse(reader.ValueSpan, out TrackColorType trackColorType))
                settings.TrackColorType = trackColorType;
            else if (propertyName.SequenceEqual("trackColor"u8) && Color.TryFromRgba(reader.ValueSpan, out Color trackColor))
                settings.TrackColor = trackColor;
            else if (propertyName.SequenceEqual("secondaryTrackColor"u8) && Color.TryFromRgba(reader.ValueSpan, out Color secondaryTrackColor))
                settings.SecondaryTrackColor = secondaryTrackColor;
            else if (propertyName.SequenceEqual("trackColorAnimDuration"u8))
                settings.TrackColorAnimDuration = reader.GetSingle();
            else if (propertyName.SequenceEqual("trackColorPulse"u8) && EnumConverter.TryParse(reader.ValueSpan, out TrackColorPulse trackColorPulse))
                settings.TrackColorPulse = trackColorPulse;
            else if (propertyName.SequenceEqual("trackPulseLength"u8))
                settings.TrackPulseLength = reader.GetInt32();
            else if (propertyName.SequenceEqual("trackStyle"u8) && EnumConverter.TryParse(reader.ValueSpan, out TrackStyle trackStyle))
                settings.TrackStyle = trackStyle;
            else if (propertyName.SequenceEqual("trackTexture"u8))
                settings.TrackTexture = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("trackTextureScale"u8))
                settings.TrackTextureScale = reader.GetInt16();
            else if (propertyName.SequenceEqual("trackGlowIntensity"u8))
                settings.TrackGlowIntensity = reader.GetInt32();
            else if (propertyName.SequenceEqual("trackAnimation"u8) && EnumConverter.TryParse(reader.ValueSpan, out TrackAnimationType trackAnimation))
                settings.TrackAnimation = trackAnimation;
            else if (propertyName.SequenceEqual("beatsAhead"u8))
                settings.BeatsAhead = reader.GetInt32();
            else if (propertyName.SequenceEqual("trackDisappearAnimation"u8) && EnumConverter.TryParse(reader.ValueSpan, out TrackDisappearAnimationType trackDisappearAnimation))
                settings.TrackDisappearAnimation = trackDisappearAnimation;
            else if (propertyName.SequenceEqual("beatsBehind"u8))
                settings.BeatsBehind = reader.GetInt32();
            else if (propertyName.SequenceEqual("backgroundColor"u8) && Color.TryFromRgba(reader.ValueSpan, out Color backgroundColor))
                settings.BackgroundColor = backgroundColor;
            else if (propertyName.SequenceEqual("showDefaultBGIfNoImage"u8))
                settings.ShowDefaultBGIfNoImage = reader.GetBoolean();
            else if (propertyName.SequenceEqual("showDefaultBGTile"u8))
                settings.ShowDefaultBGTile = reader.GetBoolean();
            else if (propertyName.SequenceEqual("defaultBGTileColor"u8) && Color.TryFromRgba(reader.ValueSpan, out Color defaultBGTileColor))
                settings.DefaultBGTileColor = defaultBGTileColor;
            else if (propertyName.SequenceEqual("defaultBGShapeType"u8) && EnumConverter.TryParse(reader.ValueSpan, out DefaultBGTileShapeType defaultBGShapeType))
                settings.DefaultBGTileShapeType = defaultBGShapeType;
            else if (propertyName.SequenceEqual("defaultBGShapeColor"u8) && Color.TryFromRgba(reader.ValueSpan, out Color defaultBGShapeColor))
                settings.DefaultBGShapeColor = defaultBGShapeColor;
            else if (propertyName.SequenceEqual("bgImage"u8))
                settings.BgImage = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("bgImageColor"u8) && Color.TryFromRgba(reader.ValueSpan, out Color bgImageColor))
                settings.BgImageColor = bgImageColor;
            else if (propertyName.SequenceEqual("parallax"u8))
                settings.Parallax = pointsConverter.Read(ref reader, typeof(PointNI), options);
            else if (propertyName.SequenceEqual("bgDisplayMode"u8) && EnumConverter.TryParse(reader.ValueSpan, out BgDisplayMode bgDisplayMode))
                settings.BgDisplayMode = bgDisplayMode;
            else if (propertyName.SequenceEqual("imageSmoothing"u8))
                settings.ImageSmoothing = reader.GetBoolean();
            else if (propertyName.SequenceEqual("lockRot"u8))
                settings.LockRot = reader.GetBoolean();
            else if (propertyName.SequenceEqual("loopBG"u8))
                settings.LoopBG = reader.GetBoolean();
            else if (propertyName.SequenceEqual("scalingRatio"u8))
                settings.ScalingRatio = reader.GetInt32();
            else if (propertyName.SequenceEqual("relativeTo"u8) && EnumConverter.TryParse(reader.ValueSpan, out CameraRelativeTo cameraRelativeTo))
                settings.RelativeTo = cameraRelativeTo;
            else if (propertyName.SequenceEqual("position"u8))
                settings.Position = pointsConverter.Read(ref reader, typeof(PointNI), options);
            else if (propertyName.SequenceEqual("rotation"u8))
                settings.Rotation = reader.GetInt32();
            else if (propertyName.SequenceEqual("zoom"u8))
                settings.Zoom = reader.GetInt32();
            else if (propertyName.SequenceEqual("pulseOnFloor"u8))
                settings.PulseOnFloor = reader.GetBoolean();
            else if (propertyName.SequenceEqual("bgVideo"u8))
                settings.BgVideo = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("loopVideo"u8))
                settings.LoopVideo = reader.GetBoolean();
            else if (propertyName.SequenceEqual("vidOffset"u8))
                settings.VidOffset = reader.GetInt32();
            else if (propertyName.SequenceEqual("floorIconOutlines"u8))
                settings.FloorIconOutlines = reader.GetBoolean();
            else if (propertyName.SequenceEqual("stickToFloors"u8))
                settings.StickToFloors = reader.GetBoolean();
            else if (propertyName.SequenceEqual("planetEase"u8) && EnumConverter.TryParse(reader.ValueSpan, out EaseType planetEase))
                settings.PlanetEase = planetEase;
            else if (propertyName.SequenceEqual("planetEaseParts"u8))
                settings.PlanetEaseParts = reader.GetInt32();
            else if (propertyName.SequenceEqual("planetEasePartBehavior"u8) && EnumConverter.TryParse(reader.ValueSpan, out EasePartBehaviors planetEasePartBehavior))
                settings.PlanetEasePartBehavior = planetEasePartBehavior;
            else if (propertyName.SequenceEqual("defaultTextColor"u8) && Color.TryFromRgba(reader.ValueSpan, out Color defaultTextColor))
                settings.DefaultTextColor = defaultTextColor;
            else if (propertyName.SequenceEqual("defaultTextShadowColor"u8) && Color.TryFromRgba(reader.ValueSpan, out Color defaultTextShadowColor))
                settings.DefaultTextShadowColor = defaultTextShadowColor;
            else if (propertyName.SequenceEqual("congratsText"u8))
                settings.CongratsText = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("perfectText"u8))
                settings.PerfectText = reader.GetString() ?? string.Empty;
            else if (propertyName.SequenceEqual("legacyFlash"u8))
                settings.LegacyFlash = reader.GetBoolean();
            else if (propertyName.SequenceEqual("legacyCamRelativeTo"u8))
                settings.LegacyCamRelativeTo = reader.GetBoolean();
            else if (propertyName.SequenceEqual("legacySpriteTiles"u8))
                settings.LegacySpriteTiles = reader.GetBoolean();
            else if (propertyName.SequenceEqual("legacyTween"u8))
                settings.LegacyTween = reader.GetBoolean();
            else if (propertyName.SequenceEqual("disableV15Features"u8))
                settings.DisableV15Features = reader.GetBoolean();
            else
                reader.Skip();
        }
        reader.Read();
        return settings;
    }
    public override void Write(Utf8JsonWriter writer, Components.Settings value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("version"u8, value.Version);
        writer.WriteString("artist"u8, value.Artist);
        writer.WriteString("specialArtistType"u8, EnumConverter.ToEnumString(value.SpecialArtistType));
        writer.WriteString("artistPermission"u8, value.ArtistPermission);
        writer.WriteString("song"u8, value.Song);
        writer.WriteString("author"u8, value.Author);
        writer.WriteBoolean("separateCountdownTime"u8, value.SeparateCountdownTime);
        writer.WriteString("previewImage"u8, value.PreviewImage);
        writer.WriteString("previewIcon"u8, value.PreviewIcon);
        writer.WriteString("previewIconColor"u8, value.PreviewIconColor.ToString("rrggbb"));
        writer.WriteNumber("previewSongStart"u8, value.PreviewSongStart);
        writer.WriteNumber("previewSongDuration"u8, value.PreviewSongDuration);
        writer.WriteBoolean("seizureWarning"u8, value.SeizureWarning);
        writer.WriteString("levelDesc"u8, value.LevelDesc);
        writer.WriteString("levelTags"u8, value.LevelTags);
        writer.WriteString("artistLinks"u8, value.ArtistLinks);
        writer.WriteNumber("speedTrialAim"u8, value.SpeedTrialAim);
        writer.WriteNumber("difficulty"u8, value.Difficulty);
        writer.WritePropertyName("requiredMods"u8);
        writer.WriteStartArray();
        foreach (string? mod in value.RequiredMods)
            writer.WriteStringValue(mod);
        writer.WriteEndArray();
        writer.WriteString("songFilename"u8, value.SongFilename);
        writer.WriteNumber("bpm"u8, value.Bpm);
        writer.WriteNumber("volume"u8, value.Volume);
        writer.WriteNumber("offset"u8, value.Offset);
        writer.WriteNumber("pitch"u8, value.Pitch);
        writer.WriteString("hitsound"u8, value.Hitsound);
        writer.WriteNumber("hitsoundVolume"u8, value.HitsoundVolume);
        writer.WriteNumber("countdownTicks"u8, value.CountdownTicks);
        writer.WriteString("trackColorType"u8, EnumConverter.ToEnumString(value.TrackColorType));
        writer.WriteString("trackColor"u8, value.TrackColor.ToString("rrggbb"));
        writer.WriteString("secondaryTrackColor"u8, value.SecondaryTrackColor.ToString("rrggbb"));
        writer.WriteNumber("trackColorAnimDuration"u8, value.TrackColorAnimDuration);
        writer.WriteString("trackColorPulse"u8, EnumConverter.ToEnumString(value.TrackColorPulse));
        writer.WriteNumber("trackPulseLength"u8, value.TrackPulseLength);
        writer.WriteString("trackStyle"u8, EnumConverter.ToEnumString(value.TrackStyle));
        writer.WriteString("trackTexture"u8, value.TrackTexture);
        writer.WriteNumber("trackTextureScale"u8, value.TrackTextureScale);
        writer.WriteNumber("trackGlowIntensity"u8, value.TrackGlowIntensity);
        writer.WriteString("trackAnimation"u8, EnumConverter.ToEnumString(value.TrackAnimation));
        writer.WriteNumber("beatsAhead"u8, value.BeatsAhead);
        writer.WriteString("trackDisappearAnimation"u8, EnumConverter.ToEnumString(value.TrackDisappearAnimation));
        writer.WriteNumber("beatsBehind"u8, value.BeatsBehind);
        writer.WriteString("backgroundColor"u8, value.BackgroundColor.ToString("rrggbb"));
        writer.WriteBoolean("showDefaultBGIfNoImage"u8, value.ShowDefaultBGIfNoImage);
        writer.WriteBoolean("showDefaultBGTile"u8, value.ShowDefaultBGTile);
        writer.WriteString("defaultBGTileColor"u8, value.DefaultBGTileColor.ToString("rrggbb"));
        writer.WriteString("defaultBGShapeType"u8, EnumConverter.ToEnumString(value.DefaultBGTileShapeType));
        writer.WriteString("defaultBGShapeColor"u8, value.DefaultBGShapeColor.ToString("rrggbb"));
        writer.WriteString("bgImage"u8, value.BgImage);
        writer.WriteString("bgImageColor"u8, value.BgImageColor.ToString("rrggbb"));
        writer.WritePropertyName("parallax"u8);
        pointsConverter.Write(writer, value.Parallax, options);
        writer.WriteString("bgDisplayMode"u8, EnumConverter.ToEnumString(value.BgDisplayMode));
        writer.WriteBoolean("imageSmoothing"u8, value.ImageSmoothing);
        writer.WriteBoolean("lockRot"u8, value.LockRot);
        writer.WriteBoolean("loopBG"u8, value.LoopBG);
        writer.WriteNumber("scalingRatio"u8, value.ScalingRatio);
        writer.WriteString("relativeTo"u8, EnumConverter.ToEnumString(value.RelativeTo));
        writer.WritePropertyName("position"u8);
        pointsConverter.Write(writer, value.Position, options);
        writer.WriteNumber("rotation"u8, value.Rotation);
        writer.WriteNumber("zoom"u8, value.Zoom);
        writer.WriteBoolean("pulseOnFloor"u8, value.PulseOnFloor);
        writer.WriteString("bgVideo"u8, value.BgVideo);
        writer.WriteBoolean("loopVideo"u8, value.LoopVideo);
        writer.WriteNumber("vidOffset"u8, value.VidOffset);
        writer.WriteBoolean("floorIconOutlines"u8, value.FloorIconOutlines);
        writer.WriteBoolean("stickToFloors"u8, value.StickToFloors);
        writer.WriteString("planetEase"u8, EnumConverter.ToEnumString(value.PlanetEase));
        writer.WriteNumber("planetEaseParts"u8, value.PlanetEaseParts);
        writer.WriteString("planetEasePartBehavior"u8, EnumConverter.ToEnumString(value.PlanetEasePartBehavior));
        writer.WriteString("defaultTextColor"u8, value.DefaultTextColor.ToString("rrggbb"));
        writer.WriteString("defaultTextShadowColor"u8, value.DefaultTextShadowColor.ToString("rrggbbaa"));
        writer.WriteString("congratsText"u8, value.CongratsText);
        writer.WriteString("perfectText"u8, value.PerfectText);
        writer.WriteBoolean("legacyFlash"u8, value.LegacyFlash);
        writer.WriteBoolean("legacyCamRelativeTo"u8, value.LegacyCamRelativeTo);
        writer.WriteBoolean("legacySpriteTiles"u8, value.LegacySpriteTiles);
        writer.WriteBoolean("legacyTween"u8, value.LegacyTween);
        writer.WriteBoolean("disableV15Features"u8, value.DisableV15Features);
        writer.WriteEndObject();
    }
}
