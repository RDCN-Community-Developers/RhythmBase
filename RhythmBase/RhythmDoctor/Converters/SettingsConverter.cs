using RhythmBase.Global.Components.RichText;
using RhythmBase.Global.Extensions;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Text.EncodingExtensions;

namespace RhythmBase.RhythmDoctor.Converters;

[RDJsonConverterFor(typeof( Components.Settings))]
internal class SettingsConverter : JsonConverter<Components.Settings>
{
	public override Components.Settings? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException($"Expected StartObject token, but got {reader.TokenType}.");
		Components.Settings settings = new();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				reader.Read();
				return settings;
			}
			if (reader.TokenType != JsonTokenType.PropertyName)
			{
				throw new JsonException($"Unexpected token type: {reader.TokenType}");
			}
			ReadOnlySpan<byte> propertyName = reader.ValueSpan;
			reader.Read();
			if (propertyName.SequenceEqual("version"u8))
				settings.Version = reader.GetInt32();
			else if (propertyName.SequenceEqual("artist"u8))
				settings.Artist = reader.GetString() ?? "";
			else if (propertyName.SequenceEqual("song"u8))
				settings.Song = RDLine<RDRichStringStyle>
#if NETSTANDARD
					.Empty
#endif
					.Deserialize(reader.GetString() ?? "");
			else if (propertyName.SequenceEqual("specialArtistType"u8) && EnumConverter.TryParse(reader.ValueSpan, out SpecialArtistTypes value))
				settings.SpecialArtistType = value;
			else if (propertyName.SequenceEqual("artistPermission"u8))
				settings.ArtistPermission = reader.GetString() ?? "";
			else if (propertyName.SequenceEqual("artistLinks"u8))
				settings.ArtistLinks = reader.GetString() ?? "";
			else if (propertyName.SequenceEqual("author"u8))
				settings.Author = RDLine<RDRichStringStyle>
#if NETSTANDARD
					.Empty
#endif
					.Deserialize(reader.GetString() ?? "");
			else if (propertyName.SequenceEqual("difficulty"u8) && EnumConverter.TryParse(reader.ValueSpan, out DifficultyLevel difficulty))
				settings.Difficulty = difficulty;
			else if (propertyName.SequenceEqual("seizureWarning"u8))
				settings.SeizureWarning = reader.GetBoolean();
			else if (propertyName.SequenceEqual("previewImage"u8))
				settings.PreviewImage = reader.GetString() ?? "";
			else if (propertyName.SequenceEqual("syringeIcon"u8))
				settings.SyringeIcon = reader.GetString() ?? "";
			else if (propertyName.SequenceEqual("previewSong"u8))
				settings.PreviewSong = reader.GetString() ?? "";
			else if (propertyName.SequenceEqual("previewSongStartTime"u8))
				settings.PreviewSongStartTime = TimeSpan.FromSeconds(reader.GetSingle());
			else if (propertyName.SequenceEqual("previewSongDuration"u8))
				settings.PreviewSongDuration = TimeSpan.FromSeconds(reader.GetSingle());
			else if (propertyName.SequenceEqual("songNameHue"u8))
				settings.SongNameHueOrGrayscale = reader.GetSingle();
			else if (propertyName.SequenceEqual("songLabelGrayscale"u8))
				settings.SongLabelGrayscale = reader.GetBoolean();
			else if (propertyName.SequenceEqual("description"u8))
				settings.Description = RDLine<RDRichStringStyle>
#if NETSTANDARD
					.Empty
#endif
					.Deserialize(reader.GetString() ?? "");
			else if (propertyName.SequenceEqual("tags"u8))
				settings.Tags = RDLine<RDRichStringStyle>
#if NETSTANDARD
					.Empty
#endif
					.Deserialize(reader.GetString() ?? "");
			else if (propertyName.SequenceEqual("separate2PLevelFilename"u8))
				settings.Separate2PLevelFilename = reader.GetString() ?? "";
			else if (propertyName.SequenceEqual("canBePlayedOn"u8) && EnumConverter.TryParse(reader.ValueSpan, out LevelPlayedMode playedMode))
				settings.CanBePlayedOn = playedMode;
			else if (propertyName.SequenceEqual("firstBeatBehavior"u8) && EnumConverter.TryParse(reader.ValueSpan, out FirstBeatBehaviors firstBeatBehavior))
				settings.FirstBeatBehavior = firstBeatBehavior;
			else if (propertyName.SequenceEqual("multiplayerAppearance"u8) && EnumConverter.TryParse(reader.ValueSpan, out MultiplayerAppearances multiplayerAppearance))
				settings.MultiplayerAppearance = multiplayerAppearance;
			else if (propertyName.SequenceEqual("levelVolume"u8))
				settings.LevelVolume = reader.GetSingle();
			else if (propertyName.SequenceEqual("rankMaxMistakes"u8))
			{
				if (reader.TokenType != JsonTokenType.StartArray)
					throw new JsonException($"Expected StartArray token, but got {reader.TokenType}.");
				int[] ranks = new int[4];
				for (int i = 0; i < 4; i++)
				{
					reader.Read();
					if (reader.TokenType != JsonTokenType.Number)
						throw new JsonException($"Expected Number token, but got {reader.TokenType}.");
					ranks[i] = reader.GetInt32();
				}
				reader.Read();
				if (reader.TokenType != JsonTokenType.EndArray)
					throw new JsonException($"Expected EndArray token, but got {reader.TokenType}.");
				settings.RankMaxMistakes = ranks;
			}
			else if (propertyName.SequenceEqual("rankDescription"u8))
			{
				if (reader.TokenType != JsonTokenType.StartArray)
					throw new JsonException($"Expected StartArray token, but got {reader.TokenType}.");
				string[] descriptions = new string[6];
				for (int i = 0; i < 6; i++)
				{
					reader.Read();
					if (reader.TokenType != JsonTokenType.String)
						throw new JsonException($"Expected String token, but got {reader.TokenType}.");
					descriptions[i] = reader.GetString() ?? "";
				}
				reader.Read();
				if (reader.TokenType != JsonTokenType.EndArray)
					throw new JsonException($"Expected EndArray token, but got {reader.TokenType}.");
				settings.RankDescription = descriptions;
			}
			else if (propertyName.SequenceEqual("mods"u8))
			{
				List<string> mods = [];
				if (reader.TokenType == JsonTokenType.StartArray)
				{
					reader.Read();
					while (reader.TokenType != JsonTokenType.EndArray)
					{
						if (reader.TokenType != JsonTokenType.String)
							throw new JsonException($"Expected String token, but got {reader.TokenType}.");
						mods.Add(reader.GetString() ?? "");
						reader.Read();
					}
				}
				else if (reader.TokenType == JsonTokenType.String)
				{
					mods.Add(reader.GetString() ?? "");
				}
				else
				{
					throw new JsonException($"Expected StartArray or String token, but got {reader.TokenType}.");
				}
				settings.Mods = mods;
			}
			else
				settings._extraData[System.Text.Encoding.UTF8.GetString(propertyName)] = JsonElement.ParseValue(ref reader);
		}
		throw new JsonException("Unexpected end of JSON.");
	}
	public override Components.Settings ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return base.ReadAsPropertyName(ref reader, typeToConvert, options);
	}
	public override void Write(Utf8JsonWriter writer, Components.Settings value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WriteNumber("version"u8, value.Version);
		writer.WriteString("artist"u8, value.Artist ?? "");
		writer.WriteString("song"u8, value.Song.Serialize());
		writer.WriteString("specialArtistType"u8, EnumConverter.ToEnumString(value.SpecialArtistType));
		writer.WriteString("artistPermission"u8, value.ArtistPermission.Path ?? "");
		writer.WriteString("artistLinks"u8, value.ArtistLinks ?? "");
		writer.WriteString("author"u8, value.Author.Serialize());
		writer.WriteString("difficulty"u8, EnumConverter.ToEnumString(value.Difficulty));
		writer.WriteBoolean("seizureWarning"u8, value.SeizureWarning);
		writer.WriteString("previewImage"u8, value.PreviewImage.Path ?? "");
		writer.WriteString("syringeIcon"u8, value.SyringeIcon.Path ?? "");
		writer.WriteString("previewSong"u8, value.PreviewSong.Path ?? "");
		writer.WriteNumber("previewSongStartTime"u8, (float)value.PreviewSongStartTime.TotalSeconds);
		writer.WriteNumber("previewSongDuration"u8, (float)value.PreviewSongDuration.TotalSeconds);
		writer.WriteNumber("songNameHue"u8, value.SongNameHueOrGrayscale);
		writer.WriteBoolean("songLabelGrayscale"u8, value.SongLabelGrayscale);
		writer.WriteString("description"u8, value.Description.Serialize());
		writer.WriteString("tags"u8, value.Tags.Serialize());
		writer.WriteString("separate2PLevelFilename"u8, value.Separate2PLevelFilename ?? "");
		writer.WriteString("canBePlayedOn"u8, EnumConverter.ToEnumString(value.CanBePlayedOn));
		writer.WriteString("firstBeatBehavior"u8, EnumConverter.ToEnumString(value.FirstBeatBehavior));
		writer.WriteString("multiplayerAppearance"u8, EnumConverter.ToEnumString(value.MultiplayerAppearance));
		writer.WriteNumber("levelVolume"u8, value.LevelVolume);

		// RankMaxMistakes
		writer.WritePropertyName("rankMaxMistakes"u8);
		writer.WriteStartArray();
		if (value.RankMaxMistakes != null)
		{
			for (int i = 0; i < 4; i++)
				writer.WriteNumberValue(i < value.RankMaxMistakes.Length ? value.RankMaxMistakes[i] : 0);
		}
		else
		{
			for (int i = 0; i < 4; i++)
				writer.WriteNumberValue(0);
		}
		writer.WriteEndArray();

		// RankDescription
		writer.WritePropertyName("rankDescription"u8);
		writer.WriteStartArray();
		if (value.RankDescription != null)
		{
			for (int i = 0; i < 6; i++)
				writer.WriteStringValue(i < value.RankDescription.Length ? value.RankDescription[i] ?? "" : "");
		}
		else
		{
			for (int i = 0; i < 6; i++)
				writer.WriteStringValue("");
		}
		writer.WriteEndArray();

		// Mods
		if (value.Mods != null && value.Mods.Count > 0)
		{
			writer.WritePropertyName("mods"u8);
			writer.WriteStartArray();
			foreach (string? mod in value.Mods)
				writer.WriteStringValue(mod ?? "");
			writer.WriteEndArray();
		}

		// ExtraData
		if (value._extraData != null)
		{
			foreach (KeyValuePair<string, JsonElement> kv in value._extraData)
			{
				writer.WritePropertyName(kv.Key);
				kv.Value.WriteTo(writer);
			}
		}

		writer.WriteEndObject();
	}
}
