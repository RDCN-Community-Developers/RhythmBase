using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Events;
using RhythmBase.Adofai.Settings;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace RhythmBase.Adofai.Converters;

internal class LevelConverter : JsonConverter<ADLevel>
{
	private static readonly BaseEventConverter baseEventConverter = new();
	private static readonly SettingsConverter settingsConverter = new();
	internal string? Filepath { get; set; }
	internal ILevelReadSettings<IBaseEvent, EventType, ADBeat> ReadSettings { get; set; } = new LevelReadSettings();
	internal ILevelWriteSettings<IBaseEvent, EventType, ADBeat> WriteSettings { get; set; } = new LevelWriteSettings();

	public override ADLevel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		ADLevel level = [];
		bool isTileLoad = false;
		List<BaseTileEvent> tileEventsNotLoad = [];
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException($"Expected StartObject token, but got {reader.TokenType}.");
		reader.Read();
		while (true)
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;
			if (reader.TokenType != JsonTokenType.PropertyName)
				throw new JsonException($"Expected PropertyName token, but got {reader.TokenType}.");
			if (reader.ValueSpan.SequenceEqual("angleData"u8))
			{
				reader.Read();
				if (reader.TokenType != JsonTokenType.StartArray)
					throw new JsonException($"Expected StartArray token, but got {reader.TokenType}.");
				reader.Read();
				while (true)
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;
					if (reader.TokenType != JsonTokenType.Number)
						throw new JsonException($"Expected Number token, but got {reader.TokenType}.");
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
			else if (reader.ValueSpan.SequenceEqual("settings"u8))
			{
				reader.Read();
				level.Settings = settingsConverter.Read(ref reader, typeof(Components.Settings), options) ?? new();
				if (level.Settings.Version < MinimumSupportedVersionAdofai)
#if DEBUG
					Console.WriteLine($"Current version {level.Settings.Version} is too low.");
#else
					throw new VersionTooLowException(MinimumSupportedVersionAdofai);
#endif
			}
			else if (reader.ValueSpan.SequenceEqual("actions"u8))
			{
				reader.Read();
				if (reader.TokenType != JsonTokenType.StartArray)
					throw new JsonException($"Expected StartArray token for 'events', but got {reader.TokenType}.");
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
			else if (reader.ValueSpan.SequenceEqual("decorations"u8))
			{
				reader.Read();
				if (reader.TokenType != JsonTokenType.StartArray)
					throw new JsonException($"Expected StartArray token, but got {reader.TokenType}.");
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

	public override void Write(Utf8JsonWriter writer, ADLevel value, JsonSerializerOptions options)
	{
		using MemoryStream stream = new();
		JsonSerializerOptions localOptions = new(options)
		{
			WriteIndented = false,
		};
		byte[] bytes = GetIndentByte(writer, options.IndentCharacter, 2);
		ReadOnlySpan<byte> sl;
		writer.WriteStartObject();
		writer.WritePropertyName("angleData");
		using Utf8JsonWriter noIndentWriter = new(stream, new JsonWriterOptions { Indented = false });
		noIndentWriter.WriteStartArray();
		foreach (Tile tile in value)
		{
			if (tile.IsMidSpin)
				noIndentWriter.WriteNumberValue(Utils.Utils.MidSpinAngle);
			else
				noIndentWriter.WriteNumberValue(tile.Angle);
		}
		noIndentWriter.WriteEndArray();
		noIndentWriter.Flush();
		sl = stream.GetBuffer().AsSpan(0, (int)stream.Position);
		writer.WriteRawValue(sl);
		noIndentWriter.Reset();
		writer.WritePropertyName("settings");
		settingsConverter.Write(writer, value.Settings, options);
		writer.WriteStartArray("actions");
		foreach (Tile tile in value)
		{
			foreach (BaseTileEvent tileEvent in tile)
			{
				stream.SetLength(0);
				if (options.WriteIndented)
					stream.Write(bytes, 0, bytes.Length);
				baseEventConverter.Write(noIndentWriter, tileEvent, localOptions);
				noIndentWriter.Flush();
				sl = stream.GetBuffer().AsSpan(0, (int)stream.Position);
				writer.WriteRawValue(sl);
				noIndentWriter.Reset();
			}
		}
		writer.WriteEndArray();
		writer.WriteStartArray("decorations");
		foreach (IBaseEvent decoration in value.Decorations)
		{
			stream.SetLength(0);
			if (options.WriteIndented)
				stream.Write(bytes, 0, bytes.Length);
			baseEventConverter.Write(noIndentWriter, decoration, localOptions);
			noIndentWriter.Flush();
			sl = stream.GetBuffer().AsSpan(0, (int)stream.Position);
			writer.WriteRawValue(sl);
			noIndentWriter.Reset();
		}
		writer.WriteEndArray();
		writer.WriteEndObject();
	}
	private static byte[] GetIndentByte(Utf8JsonWriter writer, char indentChar, int indentSize) => Encoding.UTF8.GetBytes(Environment.NewLine + new string(indentChar, writer.CurrentDepth * 2));
}
