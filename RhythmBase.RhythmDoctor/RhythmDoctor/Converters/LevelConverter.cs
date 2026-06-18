using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
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
	private static readonly UnhandledPropertyHandler<ITintEvent> TintEventBorderHandler = (ref ITintEvent e, JsonElement value) =>
	{
		if (!value.TryGetInt32(out int alpha))
			return false;
		var c = e.BorderColor.Color;
		c.A = (byte)(alpha / 100f * 255);
		e.BorderColor = c;
		return true;
	};
	private static readonly UnhandledPropertyHandler<ITintEvent> TintEventTintHandler = (ref ITintEvent e, JsonElement value) =>
	{
		if (!value.TryGetInt32(out int alpha))
			return false;
		var c = e.TintColor.Color;
		c.A = (byte)(alpha / 100f * 255);
		e.TintColor = c;
		return true;
	};
	private static readonly SoundCollectionConverter soundCollectionConverter = new();
	static LevelConverter()
	{
		// Legacy fields ignored by newer versions
		UnhandledFieldRegistry.Ignore<SetClapSounds>("p1Used");
		UnhandledFieldRegistry.Ignore<SetClapSounds>("p2Used");
		UnhandledFieldRegistry.Ignore<SetClapSounds>("cpuUsed");
		UnhandledFieldHelper.RegisterForIEaseEvent("ease", (ref IEaseEvent e, JsonElement value) =>
		{
			if (value.ValueKind != JsonValueKind.String)
				return false;
			// 按演出效果映射
			e.Ease = value.GetString() switch
			{
				"InFlash" => EaseType.InQuad,
				"OutFlash" => EaseType.OutQuad,
				"Flash" or "InOutFlash" => EaseType.InOutQuad,
				_ => e.Ease
			};
			return true;
		});
		UnhandledFieldRegistry.Register<SetVFXPreset>("speed", (ref e, value) =>
		{
			float?[] xs = value.EnumerateArray().Select(x => x.ValueKind == JsonValueKind.Number && x.TryGetSingle(out float f) ? f : (float?)null).ToArray();
			if (xs.Length != 2)
				return false;
			e.Amount = (xs[0], xs[1]);
			return true;
		});
		// Tint event fields: registered via interface, covers TintRows / Tint / PaintHands
		UnhandledFieldHelper.RegisterForITintEvent("borderOpacity", TintEventBorderHandler);
		UnhandledFieldHelper.RegisterForITintEvent("tintOpacity", TintEventTintHandler);
		UnhandledFieldHelper.RegisterForITintEvent("effectSound", (ref ITintEvent _, JsonElement ___) => true);
		UnhandledFieldRegistry.Register<ShakeScreen>("shakeLevel", (ref e, value) =>
		{
			string v = value.GetString() ?? string.Empty;
			if (string.IsNullOrEmpty(v))
				return false;
			e.ShakeLevel = 0;
			return true;
		});
		UnhandledFieldRegistry.Ignore<FloatingText>("times");
		UnhandledFieldRegistry.Ignore<FloatingText>("id");
		UnhandledFieldRegistry.Ignore<AdvanceText>("id");
		UnhandledFieldRegistry.Register<FloatingText>("narrationCategory", (ref e, value) =>
		{
			if (value.ValueKind != JsonValueKind.String)
				return false;
			string? v = value.GetString();
			if (v != "Main")
				return false;
			e.NarrationCategory = NarrationCategory.Fallback;
			return true;
		});
		UnhandledFieldRegistry.Register<NarrateRowInfo>("narrateSkipBeats", (ref e, value) =>
		{
			if (value.ValueKind != JsonValueKind.String)
				return false;
			string? v = value.GetString();
			(NarrateSkipBeat v2, bool v3) v4 = v switch
			{
				"on" => (NarrateSkipBeat.On, true),
				"off" => (NarrateSkipBeat.Off, true),
				"custom" => (NarrateSkipBeat.Custom, true),
				_ => (0, false)
			};
			e.NarrateSkipBeat = v4.v2;
			return v4.v3;
		});
		UnhandledFieldRegistry.Register<SetGameSound>("soundType", (ref e, value) =>
		{
			(e.SoundType, bool v) =
				value.ValueKind is JsonValueKind.String &&
				value.GetString() is "ClapSoundP1Hold" ?
					(SoundType.ClapSoundHold, true) :
					(e.SoundType, false);
			return v;
		});

		UnhandledFieldRegistry.Ignore<NewWindowDance>("rooms");
		UnhandledFieldRegistry.Ignore<MaskRoom>("rooms");
		UnhandledFieldRegistry.Keep<NewWindowDance>("usePosition");
		UnhandledFieldRegistry.Register<AddOneshotBeat>("squareSound", (ref e, value) =>
		{
			if (value.ValueKind is not (JsonValueKind.True or JsonValueKind.False)) return false;
			e.SubdivisionSound = value.GetBoolean();
			return true;
		});
		UnhandledFieldRegistry.Register<SetGameSound>("sounds", (ref e, value) =>
		{
			if (value.ValueKind != JsonValueKind.Array)
				return false;
			Utf8JsonReader reader = new(Encoding.UTF8.GetBytes(value.GetRawText()), new());
			if(!reader.Read()) return false;
			e.Sounds = soundCollectionConverter.Read(ref reader, typeof(SoundCollection), new JsonSerializerOptions()) ?? [];
			return true;
		});
		UnhandledFieldRegistry.Register<SetVFXPreset>("xySpeed", (ref e, value) =>
		{
			if (value.ValueKind != JsonValueKind.Array)
				return false;
			float?[] xs = value.EnumerateArray().Select(x => x.ValueKind == JsonValueKind.Number && x.TryGetSingle(out float f) ? f : (float?)null).ToArray();
			if (xs.Length != 2)
				return false;
			e.Amount = (xs[0], xs[1]);
			return true;
		});
		UnhandledFieldRegistry.Ignore<PlaySound>("isCustom");
		UnhandledFieldRegistry.Ignore<MaskRoom>("contentMode");
	}

	internal LevelReadSettings ReadSettings { get; set; } = new LevelReadSettings();
	internal LevelWriteSettings WriteSettings { get; set; } = new LevelWriteSettings();
	internal string? DirectoryName { get; set; }

	public override Level? Read(ref Utf8JsonReader reader, Type typeToConvert, MetadataJsonSerializerOptions options)
	{
		reader.Read();
		Level level = [];
		JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("settings"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
				level.Settings = settingsConverter.Read(ref reader, typeof(Settings), options) ?? new();
				if (!ReadSettings.LoadAssets || string.IsNullOrEmpty(DirectoryName)) continue;
				foreach (FileReference file in level.Settings.GetAllFileReferences())
					if (!file.IsEmpty && file.IsExist(DirectoryName!))
						ReadSettings.OnFileReferenceEncountered(level, file);
			}
			else if (reader.ValueTextEquals("rows"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					Row? e = rowConverter.Read(ref reader, typeof(Row), options);
					if (e == null) continue;
					level.Rows.Add(e);
					string assPath = DirectoryName + e.Character.CustomCharacter;
					if (!ReadSettings.LoadAssets || string.IsNullOrEmpty(DirectoryName)) continue;
					foreach (FileReference file in e.Character.GetAllPossibleFileReferences())
						if (!file.IsEmpty && file.IsExist(DirectoryName!))
							ReadSettings.OnFileReferenceEncountered(level, file);
						else if (file.IsExist(assPath))
							ReadSettings.OnFileReferenceEncountered(level, DirectoryName + Path.DirectorySeparatorChar + file);
				}
			}
			else if (reader.ValueTextEquals("decorations"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					Decoration? e = decorationConverter.Read(ref reader, typeof(Decoration), options);
					if (e == null) continue;
					level.Decorations.Add(e);
					string assPath = DirectoryName + e.Character.CustomCharacter;
					if (!ReadSettings.LoadAssets || string.IsNullOrEmpty(DirectoryName)) continue;
					foreach (FileReference file in e.Character.GetAllPossibleFileReferences())
						if (!file.IsEmpty && file.IsExist(DirectoryName!))
							ReadSettings.OnFileReferenceEncountered(level, file);
						else if (file.IsExist(assPath))
							ReadSettings.OnFileReferenceEncountered(level, DirectoryName + Path.DirectorySeparatorChar + file);
				}
			}
			else if (reader.ValueTextEquals("events"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				Dictionary<int, FloatingText> floatingTexts = [];
				List<AdvanceText> advanceTexts = [];
				JsonElement[]? data = [];
				List<JsonDocument> maybeIllegalAt = [];
				int index = 0;
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					IBaseEvent? e = null;
#if DEBUG
					try
					{
						e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
						index++;
					}
					catch (Exception)
					{
						Console.WriteLine($"Current index: {index}");
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
					if (e is FloatingText ft && ft._extraData.TryGetValue("id", out JsonElement v1))
					{
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
					if (at._extraData.TryGetValue("id", out JsonElement je) &&
							je.TryGetInt32(out int targetId) &&
							floatingTexts.TryGetValue(targetId, out FloatingText? ft))
					{
						at.Parent = ft;
						ft.Children.Add(at);
						at._extraData.Remove("id");
					}
					else
					{
						if (at._extraData.TryGetValue("id", out je) &&
								je.TryGetInt32(out targetId))
							ReadSettings.OnUnreadableEventEncountered(level, JsonElement.Parse(at.ToJsonString()),
								$"AdvanceText references non-existent FloatingText id {targetId}.");
						else
							ReadSettings.OnUnreadableEventEncountered(level, JsonElement.Parse(at.ToJsonString()),
								$"AdvanceText don't has field id.");
					}
				}
			}
			else if (reader.ValueTextEquals("bookmarks"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					Bookmark e = bookmarkConverter.Read(ref reader, typeof(Bookmark), options);
					level.Bookmarks.Add(e);
				}
			}
			else if (reader.ValueTextEquals("colorPalette"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				int colorIndex = 0;
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
					string? e = reader.GetString();
					Color color = e is null ? default : Color.FromRgba(e);
					level.ColorPalette[colorIndex] = color;
					colorIndex++;
				}
			}
			else if (reader.ValueTextEquals("conditionals"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
				{
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
		using Utf8JsonWriter noIndentWriter = new(stream,
			new JsonWriterOptions { Indented = false, Encoder = options.JsonSerializerOptions.Encoder });
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

	private static byte[] GetIndentByte(Utf8JsonWriter writer, char indentChar, int indentSize) =>
		Encoding.UTF8.GetBytes(Environment.NewLine + new string(indentChar, writer.CurrentDepth * indentSize));
}