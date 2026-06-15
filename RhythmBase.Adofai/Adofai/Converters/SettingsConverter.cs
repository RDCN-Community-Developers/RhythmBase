using RhythmBase.Adofai.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Adofai.Converters;

[JsonConverterFor(typeof(Settings))]
internal class SettingsConverter : JsonConverter<Settings>
{
    private static readonly RDPointNIConverter pointsConverter = new();
    public override Settings? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"Expected StartObject token, but got {reader.TokenType}.");
		Settings settings = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"Expected PropertyName token, but got {reader.TokenType}.");
            if (reader.ValueTextEquals("version"u8))
            {
                reader.Read();
                settings.Version = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("artist"u8))
            {
                reader.Read();
                settings.Artist = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("specialArtistType"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out SpecialArtistTypes specialArtistTypes))
                settings.SpecialArtistType = specialArtistTypes;
            else if (reader.ValueTextEquals("artistPermission"u8))
            {
                reader.Read();
                settings.ArtistPermission = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("song"u8))
            {
                reader.Read();
                settings.Song = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("author"u8))
            {
                reader.Read();
                settings.Author = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("separateCountdownTime"u8))
            {
                reader.Read();
                settings.SeparateCountdownTime = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("previewImage"u8))
            {
                reader.Read();
                settings.PreviewImage = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("previewIcon"u8))
            {
                reader.Read();
                settings.PreviewIcon = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("previewIconColor"u8) && reader.Read() && Color.TryFromRgba(ref reader, out Color color))
                settings.PreviewIconColor = color;
            else if (reader.ValueTextEquals("previewSongStart"u8))
            {
                reader.Read();
                settings.PreviewSongStart = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("previewSongDuration"u8))
            {
                reader.Read();
                settings.PreviewSongDuration = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("seizureWarning"u8))
            {
                reader.Read();
                settings.SeizureWarning = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("levelDesc"u8))
            {
                reader.Read();
                settings.LevelDesc = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("levelTags"u8))
            {
                reader.Read();
                settings.LevelTags = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("artistLinks"u8))
            {
                reader.Read();
                settings.ArtistLinks = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("speedTrialAim"u8))
            {
                reader.Read();
                settings.SpeedTrialAim = reader.GetSingle();
            }
            else if (reader.ValueTextEquals("difficulty"u8))
            {
                reader.Read();
                settings.Difficulty = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("requiredMods"u8))
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException($"Expected StartArray token, but got {reader.TokenType}.");
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType != JsonTokenType.String)
                        throw new JsonException($"Expected String token, but got {reader.TokenType}.");
                    settings.RequiredMods.Add(reader.GetString() ?? string.Empty);
                }
            }
            else if (reader.ValueTextEquals("songFilename"u8))
            {
                reader.Read();
                settings.SongFilename = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("bpm"u8))
            {
                reader.Read();
                settings.Bpm = reader.GetSingle();
            }
            else if (reader.ValueTextEquals("volume"u8))
            {
                reader.Read();
                settings.Volume = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("offset"u8))
            {
                reader.Read();
                settings.Offset = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("pitch"u8))
            {
                reader.Read();
                settings.Pitch = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("hitsound"u8))
            {
                reader.Read();
                settings.Hitsound = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("hitsoundVolume"u8))
            {
                reader.Read();
                settings.HitsoundVolume = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("countdownTicks"u8))
            {
                reader.Read();
                settings.CountdownTicks = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("tileShape"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out TileShape tileShape))
                settings.TileShape = tileShape;
            else if (reader.ValueTextEquals("trackColorType"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out TrackColorType trackColorType))
                settings.TrackColorType = trackColorType;
            else if (reader.ValueTextEquals("trackColor"u8) && reader.Read() && Color.TryFromRgba(ref reader, out Color trackColor))
                settings.TrackColor = trackColor;
            else if (reader.ValueTextEquals("secondaryTrackColor"u8) && reader.Read() && Color.TryFromRgba(ref reader, out Color secondaryTrackColor))
                settings.SecondaryTrackColor = secondaryTrackColor;
            else if (reader.ValueTextEquals("trackColorAnimDuration"u8))
            {
                reader.Read();
                settings.TrackColorAnimDuration = reader.GetSingle();
            }
            else if (reader.ValueTextEquals("trackColorPulse"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out TrackColorPulse trackColorPulse))
                settings.TrackColorPulse = trackColorPulse;
            else if (reader.ValueTextEquals("trackPulseLength"u8))
            {
                reader.Read();
                settings.TrackPulseLength = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("trackStyle"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out TrackStyle trackStyle))
                settings.TrackStyle = trackStyle;
            else if (reader.ValueTextEquals("trackTexture"u8))
            {
                reader.Read();
                settings.TrackTexture = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("trackTextureScale"u8))
            {
                reader.Read();
                settings.TrackTextureScale = reader.GetInt16();
            }
            else if (reader.ValueTextEquals("trackGlowIntensity"u8))
            {
                reader.Read();
                settings.TrackGlowIntensity = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("trackAnimation"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out TrackAnimationType trackAnimation))
                settings.TrackAnimation = trackAnimation;
            else if (reader.ValueTextEquals("beatsAhead"u8))
            {
                reader.Read();
                settings.BeatsAhead = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("trackDisappearAnimation"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out TrackDisappearAnimationType trackDisappearAnimation))
                settings.TrackDisappearAnimation = trackDisappearAnimation;
            else if (reader.ValueTextEquals("beatsBehind"u8))
            {
                reader.Read();
                settings.BeatsBehind = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("backgroundColor"u8) && reader.Read() && Color.TryFromRgba(ref reader, out Color backgroundColor))
                settings.BackgroundColor = backgroundColor;
            else if (reader.ValueTextEquals("showDefaultBGIfNoImage"u8))
            {
                reader.Read();
                settings.ShowDefaultBGIfNoImage = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("showDefaultBGTile"u8))
            {
                reader.Read();
                settings.ShowDefaultBGTile = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("defaultBGTileColor"u8) && reader.Read() && Color.TryFromRgba(ref reader, out Color defaultBGTileColor))
                settings.DefaultBGTileColor = defaultBGTileColor;
            else if (reader.ValueTextEquals("defaultBGShapeType"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out DefaultBGTileShapeType defaultBGShapeType))
                settings.DefaultBGTileShapeType = defaultBGShapeType;
            else if (reader.ValueTextEquals("defaultBGShapeColor"u8) && reader.Read() && Color.TryFromRgba(ref reader, out Color defaultBGShapeColor))
                settings.DefaultBGShapeColor = defaultBGShapeColor;
            else if (reader.ValueTextEquals("bgImage"u8))
            {
                reader.Read();
                settings.BgImage = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("bgImageColor"u8) && reader.Read() && Color.TryFromRgba(ref reader, out Color bgImageColor))
                settings.BgImageColor = bgImageColor;
            else if (reader.ValueTextEquals("parallax"u8))
            {
                reader.Read();
                settings.Parallax = pointsConverter.Read(ref reader, typeof(PointNI), options);
            }
            else if (reader.ValueTextEquals("bgDisplayMode"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out BgDisplayMode bgDisplayMode))
                settings.BgDisplayMode = bgDisplayMode;
            else if (reader.ValueTextEquals("imageSmoothing"u8))
            {
                reader.Read();
                settings.ImageSmoothing = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("lockRot"u8))
            {
                reader.Read();
                settings.LockRot = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("loopBG"u8))
            {
                reader.Read();
                settings.LoopBG = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("scalingRatio"u8))
            {
                reader.Read();
                settings.ScalingRatio = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("relativeTo"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out CameraRelativeTo cameraRelativeTo))
                settings.RelativeTo = cameraRelativeTo;
            else if (reader.ValueTextEquals("position"u8))
            {
                reader.Read();
                settings.Position = pointsConverter.Read(ref reader, typeof(PointNI), options);
            }
            else if (reader.ValueTextEquals("rotation"u8))
            {
                reader.Read();
                settings.Rotation = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("zoom"u8))
            {
                reader.Read();
                settings.Zoom = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("pulseOnFloor"u8))
            {
                reader.Read();
                settings.PulseOnFloor = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("bgVideo"u8))
            {
                reader.Read();
                settings.BgVideo = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("loopVideo"u8))
            {
                reader.Read();
                settings.LoopVideo = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("vidOffset"u8))
            {
                reader.Read();
                settings.VidOffset = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("floorIconOutlines"u8))
            {
                reader.Read();
                settings.FloorIconOutlines = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("stickToFloors"u8))
            {
                reader.Read();
                settings.StickToFloors = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("planetEase"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out EaseType planetEase))
                settings.PlanetEase = planetEase;
            else if (reader.ValueTextEquals("planetEaseParts"u8))
            {
                reader.Read();
                settings.PlanetEaseParts = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("planetEasePartBehavior"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out EasePartBehaviors planetEasePartBehavior))
                settings.PlanetEasePartBehavior = planetEasePartBehavior;
            else if (reader.ValueTextEquals("defaultTextColor"u8) && reader.Read() && Color.TryFromRgba(ref reader, out Color defaultTextColor))
                settings.DefaultTextColor = defaultTextColor;
            else if (reader.ValueTextEquals("defaultTextShadowColor"u8) && reader.Read() && Color.TryFromRgba(ref reader, out Color defaultTextShadowColor))
                settings.DefaultTextShadowColor = defaultTextShadowColor;
            else if (reader.ValueTextEquals("congratsText"u8))
            {
                reader.Read();
                settings.CongratsText = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("perfectText"u8))
            {
                reader.Read();
                settings.PerfectText = reader.GetString() ?? string.Empty;
            }
            else if (reader.ValueTextEquals("legacyFlash"u8))
            {
                reader.Read();
                settings.LegacyFlash = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("legacyCamRelativeTo"u8))
            {
                reader.Read();
                settings.LegacyCamRelativeTo = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("legacySpriteTiles"u8))
            {
                reader.Read();
                settings.LegacySpriteTiles = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("legacyTween"u8))
            {
                reader.Read();
                settings.LegacyTween = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("disableV15Features"u8))
            {
                reader.Read();
                settings.DisableV15Features = reader.GetBoolean();
            }
            else
                reader.Skip();
        }
        reader.Read();
        return settings;
    }
    public override void Write(Utf8JsonWriter writer, Settings value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("version"u8, value.Version);
        writer.WriteString("artist"u8, value.Artist);
        writer.WriteString("specialArtistType"u8, value.SpecialArtistType.ToEnumString());
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
        writer.WriteString("trackColorType"u8, value.TrackColorType.ToEnumString());
        writer.WriteString("trackColor"u8, value.TrackColor.ToString("rrggbb"));
        writer.WriteString("secondaryTrackColor"u8, value.SecondaryTrackColor.ToString("rrggbb"));
        writer.WriteNumber("trackColorAnimDuration"u8, value.TrackColorAnimDuration);
        writer.WriteString("trackColorPulse"u8, value.TrackColorPulse.ToEnumString());
        writer.WriteNumber("trackPulseLength"u8, value.TrackPulseLength);
        writer.WriteString("trackStyle"u8, value.TrackStyle.ToEnumString());
        writer.WriteString("trackTexture"u8, value.TrackTexture);
        writer.WriteNumber("trackTextureScale"u8, value.TrackTextureScale);
        writer.WriteNumber("trackGlowIntensity"u8, value.TrackGlowIntensity);
        writer.WriteString("trackAnimation"u8, value.TrackAnimation.ToEnumString());
        writer.WriteNumber("beatsAhead"u8, value.BeatsAhead);
        writer.WriteString("trackDisappearAnimation"u8, value.TrackDisappearAnimation.ToEnumString());
        writer.WriteNumber("beatsBehind"u8, value.BeatsBehind);
        writer.WriteString("backgroundColor"u8, value.BackgroundColor.ToString("rrggbb"));
        writer.WriteBoolean("showDefaultBGIfNoImage"u8, value.ShowDefaultBGIfNoImage);
        writer.WriteBoolean("showDefaultBGTile"u8, value.ShowDefaultBGTile);
        writer.WriteString("defaultBGTileColor"u8, value.DefaultBGTileColor.ToString("rrggbb"));
        writer.WriteString("defaultBGShapeType"u8, value.DefaultBGTileShapeType.ToEnumString());
        writer.WriteString("defaultBGShapeColor"u8, value.DefaultBGShapeColor.ToString("rrggbb"));
        writer.WriteString("bgImage"u8, value.BgImage);
        writer.WriteString("bgImageColor"u8, value.BgImageColor.ToString("rrggbb"));
        writer.WritePropertyName("parallax"u8);
        pointsConverter.Write(writer, value.Parallax, options);
        writer.WriteString("bgDisplayMode"u8, value.BgDisplayMode.ToEnumString());
        writer.WriteBoolean("imageSmoothing"u8, value.ImageSmoothing);
        writer.WriteBoolean("lockRot"u8, value.LockRot);
        writer.WriteBoolean("loopBG"u8, value.LoopBG);
        writer.WriteNumber("scalingRatio"u8, value.ScalingRatio);
        writer.WriteString("relativeTo"u8, value.RelativeTo.ToEnumString());
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
        writer.WriteString("planetEase"u8, value.PlanetEase.ToEnumString());
        writer.WriteNumber("planetEaseParts"u8, value.PlanetEaseParts);
        writer.WriteString("planetEasePartBehavior"u8, value.PlanetEasePartBehavior.ToEnumString());
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
