using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Events;
using System.Text.Json;
using static RhythmBase.Adofai.Constants;
namespace RhythmBase.Adofai.Converters;

[JsonConverterFor(typeof(Level))]
internal class LevelConverter : MetadataJsonConverter<Level>
{
	private static readonly BaseEventConverter baseEventConverter = new();
	private static readonly SettingsConverter settingsConverter = new();
	internal string? Filepath { get; set; }
	internal LevelReadSettings ReadSettings { get; set; } = new LevelReadSettings();
	internal LevelWriteSettings WriteSettings { get; set; } = new LevelWriteSettings();

	public override Level? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		Level level = [];
		bool isTileLoad = false;
		List<BaseTileEvent> tileEventsNotLoad = [];
		reader.Read();
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		reader.Read();
		while (true)
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("angleData"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				reader.Read();
				while (true)
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.Number);
					float angle = reader.GetSingle();
					if (angle == Utils.Utils.MidSpinAngle)
						level.Add(new Tile(true));
					else
						level.Add(new Tile(angle));
					reader.Read();
				}
				reader.Read();
				isTileLoad = true;
			}
			else if (reader.ValueTextEquals("settings"u8))
			{
				reader.Read();
				level.Settings = settingsConverter.Read(ref reader, typeof(Settings), options.JsonSerializerOptions) ?? new();
				if (level.Settings.Version < MinimumSupportedVersion)
#if DEBUG
					Console.WriteLine($"Current version {level.Settings.Version} is too low.");
#else
					throw new VersionTooLowException(MinimumSupportedVersion);
#endif
			}
			else if (reader.ValueTextEquals("actions"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				reader.Read();
				while (true)
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;
					IBaseEvent? e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
					if (e == null)
						continue;
					if (e is BaseTileEvent tileE)
					{
						if (isTileLoad)
							level[tileE._floor].Add(tileE);
						else
							tileEventsNotLoad.Add(tileE);
					}
				}
				reader.Read();
			}
			else if (reader.ValueTextEquals("decorations"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				reader.Read();
				while (true)
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;
					IBaseEvent? e = baseEventConverter.Read(ref reader, typeof(BaseEvent), options) as BaseEvent;
					if (e != null)
						level.Decorations.Add(e);
				}
				reader.Read();
			}
			else
			{
				reader.Skip();
				reader.Read();
			}
		}
		reader.Read();
		return level;
	}

	public override void Write(Utf8JsonWriter writer, Level value, MetadataJsonSerializerOptions options)
	{
		using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, options);
		writer.WriteStartObject();
		writer.WritePropertyName("angleData");
		noIndentScope.WriteNoIndentObjectTo(writer, value, (writer, value, _) =>
		{
			writer.WriteStartArray();
			foreach (Tile tile in value)
			{
				if (tile.IsMidSpin)
					writer.WriteNumberValue(Utils.Utils.MidSpinAngle);
				else
					writer.WriteNumberValue(tile.Angle);
			}
			writer.WriteEndArray();
		});
		writer.WritePropertyName("settings");
		settingsConverter.Write(writer, value.Settings, options.JsonSerializerOptions);
		writer.WriteStartArray("actions");
		noIndentScope.WriteNoIndentArrayTo(options.WriteIndented, false, writer, value.SelectMany(i => i.Cast<IBaseEvent>()), baseEventConverter.Write);
		writer.WriteEndArray();
		writer.WriteStartArray("decorations");
		//noIndentScope.WriteNoIndentArrayTo(options.WriteIndented, false, writer, value.Decorations, baseEventConverter.Write);
		foreach (var deco in value.Decorations)
			baseEventConverter.Write(writer, deco, options);
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
