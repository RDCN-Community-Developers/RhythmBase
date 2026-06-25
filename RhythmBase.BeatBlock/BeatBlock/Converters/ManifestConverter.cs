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
			if (reader.ValueTextEquals("defaultVariant"u8) && reader.Read())
				level.DefaultVariant = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
			else if (reader.ValueTextEquals("metadata"u8) && reader.Read())
			{
				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndObject)
						break;
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
					if (reader.ValueTextEquals("artist"u8) && reader.Read())
						level.Metadata.Artist = reader.GetString() ?? "";
					else if (reader.ValueTextEquals("artistLink"u8) && reader.Read())
						level.Metadata.ArtistLink = reader.GetString() ?? "";
					else if (reader.ValueTextEquals("bg"u8) && reader.Read())
						level.Metadata.IsBackgroundEnabled = reader.GetBoolean();
					else if (reader.ValueTextEquals("charter"u8) && reader.Read())
						level.Metadata.Charter = reader.GetString() ?? "";
					else if (reader.ValueTextEquals("description"u8) && reader.Read())
						level.Metadata.Description = reader.GetString() ?? "";
					else if (reader.ValueTextEquals("difficulty"u8) && reader.Read())
						level.Metadata.Difficulty = reader.GetInt32();
					else if (reader.ValueTextEquals("startLoop"u8) && reader.Read())
						level.Metadata.StartLoop = reader.GetSingle();
					else if (reader.ValueTextEquals("endLoop"u8) && reader.Read())
						level.Metadata.EndLoop = reader.GetSingle();
					else if (reader.ValueTextEquals("lightWarning"u8) && reader.Read())
						level.Metadata.LightWarning = reader.GetBoolean();
					else if (reader.ValueTextEquals("loopPointsEnable"u8) && reader.Read())
						level.Metadata.LoopPointsEnable = reader.GetBoolean();
					else if (reader.ValueTextEquals("lyricsWarning"u8) && reader.Read())
						level.Metadata.LyricsWarning = reader.GetBoolean();
					else if (reader.ValueTextEquals("songName"u8) && reader.Read())
						level.Metadata.SongName = reader.GetString() ?? "";
					else if (reader.ValueTextEquals("source"u8) && reader.Read())
						level.Metadata.Source = reader.GetString() ?? "";
					else if (reader.ValueTextEquals("bgData"u8) && reader.Read())
					{
						JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
						while (reader.Read())
						{
							if (reader.TokenType == JsonTokenType.EndObject)
								break;
							JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
							if (reader.ValueTextEquals("redChannel"u8) && reader.Read())
								level.Metadata.BackgroundData.RedChannel = TypeConverterRegistry.Read<Color>(ref reader, options);
							else if (reader.ValueTextEquals("greenChannel"u8) && reader.Read())
								level.Metadata.BackgroundData.GreenChannel = TypeConverterRegistry.Read<Color>(ref reader, options);
							else if (reader.ValueTextEquals("blueChannel"u8) && reader.Read())
								level.Metadata.BackgroundData.BlueChannel = TypeConverterRegistry.Read<Color>(ref reader, options);
							else if (reader.ValueTextEquals("yellowChannel"u8) && reader.Read())
								level.Metadata.BackgroundData.YellowChannel = TypeConverterRegistry.Read<Color>(ref reader, options);
							else if (reader.ValueTextEquals("cyanChannel"u8) && reader.Read())
								level.Metadata.BackgroundData.CyanChannel = TypeConverterRegistry.Read<Color>(ref reader, options);
							else if (reader.ValueTextEquals("magentaChannel"u8) && reader.Read())
								level.Metadata.BackgroundData.MagentaChannel = TypeConverterRegistry.Read<Color>(ref reader, options);
							else if (reader.ValueTextEquals("hideCranky"u8) && reader.Read())
								level.Metadata.BackgroundData.HideCranky = reader.GetBoolean();
							else if (reader.ValueTextEquals("image"u8) && reader.Read())
								level.Metadata.BackgroundData.Image = TypeConverterRegistry.Read<FileReference>(ref reader, options);
							else if (reader.ValueTextEquals("resultsImage"u8) && reader.Read())
								level.Metadata.BackgroundData.ResultsImage = TypeConverterRegistry.Read<FileReference>(ref reader, options);
							else
								reader.Skip();
						}
					}
					else
						reader.Skip();
				}
			}
			else if (reader.ValueTextEquals("properties"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndObject)
						break;
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
					if (reader.ValueTextEquals("formatVersion"u8) && reader.Read())
						level.Properties.FormatVersion = reader.GetInt32();
					else if (reader.ValueTextEquals("offset"u8) && reader.Read())
						level.Properties.Offset = reader.GetInt32();
					else if (reader.ValueTextEquals("startingBeat"u8) && reader.Read())
						level.Properties.StartingBeat = reader.GetInt32();
					else if (reader.ValueTextEquals("loadBeat"u8) && reader.Read())
						level.Properties.LoadBeat = reader.GetInt32();
					else if (reader.ValueTextEquals("formatVersion"u8) && reader.Read())
						level.Properties.FormatVersion = reader.GetInt32();
					else
						reader.Skip();
				}
			}
			else if (reader.ValueTextEquals("variants"u8) && reader.Read())
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
						if (reader.ValueTextEquals("charter"u8) && reader.Read())
							variant.Charter = reader.GetString() ?? "";
						else if (reader.ValueTextEquals("difficulty"u8) && reader.Read())
						// https://github.com/DPS2004/Beatblock-issues/issues/606
							variant.Difficulty =
								reader.TryGetInt32(out int intValue) ? intValue :
								reader.TryGetSingle(out float floatValue) ? (int)floatValue :
								0;						
						else if (reader.ValueTextEquals("display"u8) && reader.Read())
							variant.Display = reader.GetString() ?? "";
						else if (reader.ValueTextEquals("extra"u8) && reader.Read())
							variant.Extra = reader.GetBoolean();
						else if (reader.ValueTextEquals("hidden"u8) && reader.Read())
							variant.Hidden = reader.GetBoolean();
						else if (reader.ValueTextEquals("name"u8) && reader.Read())
							variant.Name = reader.GetString() ?? "";
						else if (reader.ValueTextEquals("slot"u8) && reader.Read())
							index = reader.GetInt32();
						else if (reader.ValueTextEquals("levelFile"u8) && reader.Read())
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
		MetadataJsonSerializerOptions localOptions = new()
		{
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
		noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.RedChannel, TypeConverterRegistry.Write);
		writer.WritePropertyName("greenChannel");
		noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.GreenChannel, TypeConverterRegistry.Write);
		writer.WritePropertyName("blueChannel");
		noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.BlueChannel, TypeConverterRegistry.Write);
		writer.WritePropertyName("yellowChannel");
		noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.YellowChannel, TypeConverterRegistry.Write);
		writer.WritePropertyName("cyanChannel");
		noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.CyanChannel, TypeConverterRegistry.Write);
		writer.WritePropertyName("magentaChannel");
		noIndentScope.WriteNoIndentObjectTo(writer, value.Metadata.BackgroundData.MagentaChannel, TypeConverterRegistry.Write);
		writer.WriteBoolean("hideCranky"u8, value.Metadata.BackgroundData.HideCranky);
		writer.WritePropertyName("image");
		TypeConverterRegistry.Write(writer, value.Metadata.BackgroundData.Image, options);
		writer.WritePropertyName("resultsImage");
		TypeConverterRegistry.Write(writer, value.Metadata.BackgroundData.ResultsImage, options);
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
		noIndentScope.WriteNoIndentArrayTo(options, writer, value.Variants, (w, v, o) =>
		{
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
