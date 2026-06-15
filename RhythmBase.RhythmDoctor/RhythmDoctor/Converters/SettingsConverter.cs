using RhythmBase.Global.Components.RichText;
using RhythmBase.RhythmDoctor.Components;
using System.Text.Json;
using RhythmBase.Global.Converters;
using static System.Text.EncodingExtensions;
using System.Text;

namespace RhythmBase.RhythmDoctor.Converters;

//public static class SettingsTrigger
//{
//	public static event Action<string>? OnTrigger;
//	public static void Trigger(string message)
//	{
//		OnTrigger?.Invoke(message);
//	}
//}

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
			if (reader.ValueTextEquals("version"u8))
			{
				reader.Read();
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
			else if (reader.ValueTextEquals("artist"u8) && reader.Read())
				settings.Artist = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("song"u8) && reader.Read())
				settings.Song = RichLine<RichStringStyle>
#if NETSTANDARD
						.Empty
#endif
						.Deserialize(reader.GetString() ?? "");
			else if (reader.ValueTextEquals("specialArtistType"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out SpecialArtistTypes value))
				settings.SpecialArtistType = value;
			else if (reader.ValueTextEquals("artistPermission"u8) && reader.Read())
				settings.ArtistPermission = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("artistLinks"u8) && reader.Read())
				settings.ArtistLinks = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("author"u8) && reader.Read())
				settings.Author = RichLine<RichStringStyle>
#if NETSTANDARD
						.Empty
#endif
						.Deserialize(reader.GetString() ?? "");
			else if (reader.ValueTextEquals("difficulty"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out DifficultyLevel difficulty))
				settings.Difficulty = difficulty;
			else if (reader.ValueTextEquals("seizureWarning"u8) && reader.Read())
				settings.SeizureWarning = reader.GetBoolean();
			else if (reader.ValueTextEquals("previewImage"u8) && reader.Read())
				settings.PreviewImage = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("syringeIcon"u8) && reader.Read())
				settings.SyringeIcon = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("previewSong"u8) && reader.Read())
				settings.PreviewSong = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("previewSongStartTime"u8) && reader.Read())
				settings.PreviewSongStartTime = TimeSpan.FromSeconds(reader.GetSingle());
			else if (reader.ValueTextEquals("previewSongDuration"u8) && reader.Read())
				settings.PreviewSongDuration = TimeSpan.FromSeconds(reader.GetSingle());
			else if (reader.ValueTextEquals("songNameHue"u8) && reader.Read())
				settings.SongNameHueOrGrayscale = reader.GetSingle();
			else if (reader.ValueTextEquals("songLabelGrayscale"u8) && reader.Read())
				settings.SongLabelGrayscale = reader.GetBoolean();
			else if (reader.ValueTextEquals("description"u8) && reader.Read())
				settings.Description = RichLine<RichStringStyle>
#if NETSTANDARD
						.Empty
#endif
						.Deserialize(reader.GetString() ?? "");
			else if (reader.ValueTextEquals("tags"u8) && reader.Read())
				settings.Tags = RichLine<RichStringStyle>
#if NETSTANDARD
						.Empty
#endif
						.Deserialize(reader.GetString() ?? "");
			else if (reader.ValueTextEquals("separate2PLevelFilename"u8) && reader.Read())
				settings.Separate2PLevelFilename = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("canBePlayedOn"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out LevelPlayedMode playedMode))
				settings.CanBePlayedOn = playedMode;
			else if (reader.ValueTextEquals("firstBeatBehavior"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out FirstBeatBehaviors firstBeatBehavior))
				settings.FirstBeatBehavior = firstBeatBehavior;
			else if (reader.ValueTextEquals("multiplayerAppearance"u8) && reader.Read() && EnumConverter.TryParse(ref reader, out MultiplayerAppearances multiplayerAppearance))
				settings.MultiplayerAppearance = multiplayerAppearance;
			else if (reader.ValueTextEquals("levelVolume"u8) && reader.Read())
				settings.LevelVolume = reader.GetSingle();
			else if (reader.ValueTextEquals("rankMaxMistakes"u8))
			{
				reader.Read();
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
						throw new JsonException($"Unexpected token in rankMaxMistakes array: {reader.TokenType} \"{reader.GetString()}\"");
				}
				settings.RankMaxMistakes = ranks;
			}
			else if (reader.ValueTextEquals("rankDescription"u8))
			{
				reader.Read();
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
						throw new JsonException($"Unexpected token in rankDescription array: {reader.TokenType} \"{reader.GetString()}\"");
				}
				settings.RankDescription = descriptions;
			}
			else if (reader.ValueTextEquals("mods"u8))
			{
				reader.Read();
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
			else if (reader.ValueTextEquals("customClass"u8) && reader.Read())
			{
				settings.CustomClass = reader.GetString();
			}
			else
			{
				var key = reader.GetString() ?? "";
				reader.Read();
				var elem = JsonElement.ParseValue(ref reader);
				//SettingsTrigger.Trigger($"ExtraData: {settings.Song} {key} = \"{elem}\"");
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

		if(value.CustomClass != null)
			writer.WriteString("customClass"u8, value.CustomClass);

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
