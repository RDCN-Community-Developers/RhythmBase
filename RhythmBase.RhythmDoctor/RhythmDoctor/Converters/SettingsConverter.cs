using RhythmBase.Global.Components.RichText;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using RhythmBase.Global.Converters;
using static System.Text.EncodingExtensions;
using System.Text;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Settings))]
internal class SettingsConverter : MetadataJsonConverter<Settings>
{
	public override Settings? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		Settings settings = new();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			ReadOnlySpan<byte> propertyName = reader.ValueSpan;
			reader.Read();
			if (propertyName.SequenceEqual("version"u8))
			{
				if (options.Strictness == JsonStrictness.Strict)
					settings.Version = reader.GetInt32();
				else
				{
					settings.Version =
						(reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int v1)) ? v1 :
						int.TryParse(reader.GetString(), out v1) ? v1 :
						0;
				}
				options.Version = settings.Version;
			}
			else if (propertyName.SequenceEqual("artist"u8))
				settings.Artist = reader.GetString() ?? "";
			else if (propertyName.SequenceEqual("song"u8))
				settings.Song = RichLine<RichStringStyle>
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
				settings.Author = RichLine<RichStringStyle>
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
				settings.Description = RichLine<RichStringStyle>
#if NETSTANDARD
						.Empty
#endif
						.Deserialize(reader.GetString() ?? "");
			else if (propertyName.SequenceEqual("tags"u8))
				settings.Tags = RichLine<RichStringStyle>
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
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				int[] ranks = new int[4];
				for (int i = 0; i < 4; i++)
				{
					reader.Read();
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.Number);
					ranks[i] = reader.GetInt32();
				}
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					if (options.Strictness == JsonStrictness.Strict)
						throw new JsonException($"Unexpected token in rankMaxMistakes array: {reader.TokenType} \"{Encoding.UTF8.GetString(reader.ValueSpan)}\"");
				}
				settings.RankMaxMistakes = ranks;
			}
			else if (propertyName.SequenceEqual("rankDescription"u8))
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				string[] descriptions = new string[6];
				for (int i = 0; i < 6; i++)
				{
					reader.Read();
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.String);
					descriptions[i] = reader.GetString() ?? "";
				}
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					if (options.Strictness == JsonStrictness.Strict)
						throw new JsonException($"Unexpected token in rankDescription array: {reader.TokenType} \"{Encoding.UTF8.GetString(reader.ValueSpan)}\"");
				}
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
						JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.String);
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
			{
				var key = Encoding.UTF8.GetString(propertyName);
				var elem = JsonElement.ParseValue(ref reader);
				Console.WriteLine($"Unknown: {key} {elem.GetRawText()}");
				settings._extraData[key] = elem;
			}
		}
		return settings;
	}
	public override void Write(Utf8JsonWriter writer, Settings value, MetadataJsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WriteNumber("version"u8, value.Version);
		writer.WriteString("artist"u8, value.Artist ?? "");
		writer.WriteString("song"u8, value.Song.Serialize());
		writer.WriteString("specialArtistType"u8, value.SpecialArtistType.ToEnumString());
		writer.WriteString("artistPermission"u8, value.ArtistPermission.Path ?? "");
		writer.WriteString("artistLinks"u8, value.ArtistLinks ?? "");
		writer.WriteString("author"u8, value.Author.Serialize());
		writer.WriteString("difficulty"u8, value.Difficulty.ToEnumString());
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
		writer.WriteString("canBePlayedOn"u8, value.CanBePlayedOn.ToEnumString());
		writer.WriteString("firstBeatBehavior"u8, value.FirstBeatBehavior.ToEnumString());
		writer.WriteString("multiplayerAppearance"u8, value.MultiplayerAppearance.ToEnumString());
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
