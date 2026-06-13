using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Converters;

[JsonConverterFor(typeof(Level))]
internal sealed class LevelConverter : MetadataJsonConverter<Level>
{
	private static readonly SettingsConverter settingsConverter = new();
	private static readonly RowConverter rowConverter = new();
	private static readonly DecorationConverter decorationConverter = new();
	private static readonly BaseEventConverter baseEventConverter = new();
	private static readonly BookmarkConverter bookmarkConverter = new();
	private static readonly ConditionalConverter conditionalConverter = new();
	internal LevelReadSettings ReadSettings { get; set; } = new LevelReadSettings();
	internal LevelWriteSettings WriteSettings { get; set; } = new LevelWriteSettings();
	internal string? DirectoryName { get; set; }
	public override Level? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		reader.Read();
		baseEventConverter.WithReadSettings(ReadSettings);
		Level level = [];
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueSpan.SequenceEqual("settings"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
				level.Settings = settingsConverter.Read(ref reader, typeof(Settings), options) ?? new();
				if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
					foreach (FileReference file in level.Settings.GetAllFileReferences())
						if (!file.IsEmpty && file.IsExist(DirectoryName!))
							ReadSettings.OnFileReferenceEncountered(level, file);
			}
			else if (reader.ValueSpan.SequenceEqual("rows"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;
					Row? e = rowConverter.Read(ref reader, typeof(Row), options);
					if (e != null)
					{
						level.Rows.Add(e);
						string assPath = DirectoryName + e.Character.CustomCharacter;
						if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
							foreach (FileReference file in e.Character.GetAllPossibleFileReferences())
								if (!file.IsEmpty && file.IsExist(DirectoryName!))
									ReadSettings.OnFileReferenceEncountered(level, file);
								else if (file.IsExist(assPath))
									ReadSettings.OnFileReferenceEncountered(level, DirectoryName + Path.DirectorySeparatorChar + file);
					}
				}
			}
			else if (reader.ValueSpan.SequenceEqual("decorations"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;
					Decoration? e = decorationConverter.Read(ref reader, typeof(Decoration), options);
					if (e != null)
					{
						level.Decorations.Add(e);
						string assPath = DirectoryName + e.Character.CustomCharacter;
						if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
							foreach (FileReference file in e.Character.GetAllPossibleFileReferences())
								if (!file.IsEmpty && file.IsExist(DirectoryName!))
									ReadSettings.OnFileReferenceEncountered(level, file);
								else if (file.IsExist(assPath))
									ReadSettings.OnFileReferenceEncountered(level, DirectoryName + Path.DirectorySeparatorChar + file);
					}
				}
			}
			else if (reader.ValueSpan.SequenceEqual("events"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				Dictionary<int, FloatingText> floatingTexts = [];
				List<AdvanceText> advanceTexts = [];
				JsonElement[]? data = [];
				List<JsonDocument> maybeIllegalAt = [];
				reader.Read();
				int index = 0;
				while (true)
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;
					IBaseEvent? e = null;
#if DEBUG
					try
					{
						e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
						index++;
					}
					catch (Exception)
					{
						Debug.Print($"Current index: {index}");
						throw;
					}
#else
                    Utf8JsonReader checkPoint = reader;
                    try
                    {
                        e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
                    }
                    catch (JsonException)
                    {
                        level.Dispose();
                        throw;
                    }
                    catch (Exception ex)
                    {
                        JsonElement element = JsonElement.ParseValue(ref checkPoint);
                        ReadSettings.OnUnreadableEventEncountered(level, element, ex.Message);
                        continue;
                    }
#endif
					if (e == null)
						continue;
					level.Add(e);
					if (e is FloatingText ft)
					{
						JsonElement v1 = ft["id"];
						int v2 = v1.GetInt32();
						floatingTexts[v2] = ft;
						ft._extraData.Remove("id");
					}
					else if (e is AdvanceText at)
						advanceTexts.Add(at);
					if (e is IFileEvent fe)
					{
						if (ReadSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
							foreach (FileReference file in fe.Files)
								if (!file.IsEmpty && file.IsExist(DirectoryName!))
									ReadSettings.OnFileReferenceEncountered(level, file);
					}
				}
				foreach (AdvanceText? at in advanceTexts)
				{
					int targetId = at["id"].GetInt32();
					if (floatingTexts.TryGetValue(targetId, out FloatingText? ft))
					{
						at.Parent = ft;
						ft.Children.Add(at);
						at._extraData.Remove("id");
					}
					else
					{
						ReadSettings.OnUnreadableEventEncountered(level, JsonElement.Parse(at.ToJsonString()), $"AdvanceText references non-existent FloatingText id {targetId}.");
					}
				}
			}
			else if (reader.ValueSpan.SequenceEqual("bookmarks"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				reader.Read();
				while (true)
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;
					Bookmark e = bookmarkConverter.Read(ref reader, typeof(Bookmark), options);
					level.Bookmarks.Add(e);
					reader.Read();
				}
			}
			else if (reader.ValueSpan.SequenceEqual("colorPalette"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				int colorIndex = 0;
				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndArray)
					{
						break;
					}
					string? e = reader.GetString();
					Color color = e is null ? default : Color.FromRgba(e);
					level.ColorPalette[colorIndex] = color;
					colorIndex++;
				}
			}
			else if (reader.ValueSpan.SequenceEqual("conditionals"u8))
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				reader.Read();
				while (true)
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;
					BaseConditional? e = conditionalConverter.Read(ref reader, typeof(BaseConditional), options);
					if (e != null)
						level.Conditionals.Add(e);
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
		baseEventConverter.WithWriteSettings(WriteSettings);
		using MemoryStream stream = new();
		MetadataJsonSerializerOptions localOptions = new()
		{
			JsonSerializerOptions = new(options.JsonSerializerOptions)
			{ WriteIndented = false, }
		};
		byte[] bytes = GetIndentByte(writer, options.JsonSerializerOptions.IndentCharacter, 2);
		ReadOnlySpan<byte> sl;
		writer.WriteStartObject();
		writer.WritePropertyName("settings");
		settingsConverter.Write(writer, value.Settings, options);
		if (WriteSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
			foreach (FileReference fr in value.Settings.GetAllFileReferences())
				if (!fr.IsEmpty && fr.IsExist(DirectoryName!))
					WriteSettings.OnFileReferenceEncountered(value, fr);

		writer.WritePropertyName("rows");
		writer.WriteStartArray();
		using Utf8JsonWriter noIndentWriter = new(stream, new JsonWriterOptions { Indented = false, Encoder = options.JsonSerializerOptions.Encoder });
		using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, localOptions);
		noIndentScope.WriteNoIndentArrayTo(options, writer, value.Rows, (writer, row, options) =>
		{
			rowConverter.Write(writer, row, options);
			string assPath = Path.Combine(DirectoryName ?? "", row.Character.CustomCharacter ?? "");
			if (WriteSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
				foreach (FileReference file in row.Character.GetAllPossibleFileReferences())
					if (!file.IsEmpty && file.IsExist(DirectoryName!))
						WriteSettings.OnFileReferenceEncountered(value, file);
					else if (file.IsExist(assPath))
						WriteSettings.OnFileReferenceEncountered(value, DirectoryName + Path.DirectorySeparatorChar + file);
		});
		writer.WriteEndArray();
		writer.WritePropertyName("decorations");
		writer.WriteStartArray();
		noIndentScope.WriteNoIndentArrayTo(options, writer, value.Decorations, (writer, decoration, options) =>
		{
			decorationConverter.Write(writer, decoration, options);
			string assPath = Path.Combine(DirectoryName ?? "", decoration.Character.CustomCharacter ?? "");
			if (WriteSettings.LoadAssets && !string.IsNullOrEmpty(DirectoryName))
				foreach (FileReference file in decoration.Character.GetAllPossibleFileReferences())
					if (!file.IsEmpty && file.IsExist(DirectoryName!))
						WriteSettings.OnFileReferenceEncountered(value, file);
					else if (file.IsExist(assPath))
						WriteSettings.OnFileReferenceEncountered(value, DirectoryName + Path.DirectorySeparatorChar + file);
		});
		writer.WriteEndArray();
		writer.WritePropertyName("events");
		writer.WriteStartArray();
		noIndentScope.WriteNoIndentArrayTo(options.WriteIndented, false, writer, value, (writer, e, options) =>
		{
			baseEventConverter.Write(writer, e, options);
			if (WriteSettings.LoadAssets && e is IFileEvent fe && !string.IsNullOrEmpty(DirectoryName))
				foreach (FileReference file in fe.Files)
					if (!file.IsEmpty && file.IsExist(DirectoryName!))
						WriteSettings.OnFileReferenceEncountered(value, file);
		});
		writer.WriteEndArray();
		writer.WritePropertyName("bookmarks");
		writer.WriteStartArray();
		foreach (Bookmark bookmark in value.Bookmarks)
			bookmarkConverter.Write(writer, bookmark, localOptions);
		writer.WriteEndArray();
		writer.WritePropertyName("colorPalette");
		writer.WriteStartArray();
		foreach (Color color in value.ColorPalette)
			writer.WriteStringValue(color.ToString("RRGGBBAA"));
		writer.WriteEndArray();
		writer.WritePropertyName("conditionals");
		writer.WriteStartArray();
		noIndentScope.WriteNoIndentArrayTo(options, writer, value.Conditionals, conditionalConverter.Write);
		writer.WriteEndArray();
		writer.WriteEndObject();
	}

	private static byte[] GetIndentByte(Utf8JsonWriter writer, char indentChar, int indentSize) => Encoding.UTF8.GetBytes(Environment.NewLine + new string(indentChar, writer.CurrentDepth * indentSize));
}