using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Events;
using System.Text.Json;
using static RhythmBase.Adofai.Constants;
namespace RhythmBase.Adofai.Serialization;

[JsonConverterFor(typeof(Level))]
internal class LevelConverter : MetadataJsonConverter<Level>
{
	private static readonly BaseEventConverter baseEventConverter = new();
	private static readonly SettingsConverter settingsConverter = new();
	internal string? Filepath { get; set; }
	internal string? DirectoryName { get; set; }
	internal LevelReadConfig ReadSettings { get; set; } = new LevelReadConfig();
	internal LevelWriteConfig WriteSettings { get; set; } = new LevelWriteConfig();

	public override Level? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		ReadSettings = options.ReadSettings ?? ReadSettings;
		DirectoryName = options.DirectoryName ?? DirectoryName;
		Level level = [];
		bool isTileLoad = false;
		List<IBaseEvent> tileEventsNotLoad = [];
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
					if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName) && e is IFileEvent fileEvent)
						foreach (FileReference file in fileEvent.Files)
							if (!file.IsEmpty && file.IsExist(DirectoryName!))
								ReadSettings.OnFileReferenceEncountered(level, file);
					if (e is BaseTileEvent)
					{
						if (isTileLoad)
							level[GetFloor(e)].Add(e);
						else
							tileEventsNotLoad.Add(e);
					}
					else if (e is BaseEvent free)
					{
						level.Add(free);
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
					{
						level.Decorations.Add(e);
						if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName) && e is IFileEvent decoFileEvent)
							foreach (FileReference file in decoFileEvent.Files)
								if (!file.IsEmpty && file.IsExist(DirectoryName!))
									ReadSettings.OnFileReferenceEncountered(level, file);
					}
				}
				reader.Read();
			}
			else
			{
				reader.Skip();
				reader.Read();
			}
		}
		// Seed the tick-authoritative position of free events from the floor/angleOffset stored in the file.
		level.Calculator.MarkDirty();
		level.Calculator.Refresh();
		foreach (BaseEvent free in level.FreeEvents)
		{
			if (free._floor < 0)
				continue;
			float tick = level.Calculator.TileOffsetToTick(free._floor, free.AngleOffset);
			free.TickTime = level.Calculator.TickOf(tick);
		}
		reader.Read();
		return level;
	}

	private static int GetFloor(IBaseEvent e) => e switch
	{
		BaseEvent be => be._floor,
		BaseTileEvent bte => bte._floor,
		_ => -1
	};

	public override void Write(Utf8JsonWriter writer, Level value, MetadataJsonSerializerOptions options)
	{
		WriteSettings = options.WriteSettings ?? WriteSettings;
		DirectoryName = options.DirectoryName ?? DirectoryName;
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
		// Free events carry a tick-authoritative position; resolve their floor/angleOffset on export.
		value.Calculator.MarkDirty();
		value.Calculator.Refresh();
		foreach (BaseEvent free in value.FreeEvents)
		{
			(int floor, float angleOffset) = value.Calculator.TickToTileOffset(free.TickTime.Tick);
			free._floor = floor;
			free.AngleOffset = angleOffset;
		}
		IEnumerable<IBaseEvent> actions = value.SelectMany(i => i.Cast<IBaseEvent>())
			.Concat(value.FreeEvents)
			.OrderBy(GetFloor)
			.ThenBy(e => e is BaseEvent angleEvent ? angleEvent.AngleOffset : 0f);
		noIndentScope.WriteNoIndentArrayTo(options.WriteIndented, false, writer, actions, (writer, e, options) =>
		{
			baseEventConverter.Write(writer, e, options);
			if (WriteSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName) && e is IFileEvent fileEvent)
				foreach (FileReference file in fileEvent.Files)
					if (!file.IsEmpty && file.IsExist(DirectoryName!))
						WriteSettings.OnFileReferenceEncountered(value, file);
		});
		writer.WriteEndArray();
		writer.WriteStartArray("decorations");
		foreach (var deco in value.Decorations)
		{
			baseEventConverter.Write(writer, deco, options);
			if (WriteSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName) && deco is IFileEvent decoFileEvent)
				foreach (FileReference file in decoFileEvent.Files)
					if (!file.IsEmpty && file.IsExist(DirectoryName!))
						WriteSettings.OnFileReferenceEncountered(value, file);
		}
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
}
