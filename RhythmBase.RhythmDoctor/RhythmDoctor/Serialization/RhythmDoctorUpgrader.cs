using RhythmBase.Global.Serialization;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace RhythmBase.RhythmDoctor.Serialization;

/// <summary>
/// Registry of the RhythmDoctor JSON upgraders and the entry point that inserts the upgrade pass
/// between <see cref="JsonCompactStream"/> and deserialization.
/// </summary>
internal sealed class RhythmDoctorUpgrader : RhythmDoctorUpgraderBase
{
	private const string EventsPath = "$.events[*]";

	private static readonly RhythmDoctorUpgrader _instance = new();

	private static readonly Dictionary<string, string> EventTypeAliases = new(StringComparer.Ordinal)
	{
		["ReorderSprite"] = nameof(EventType.ReorderDecoration),
	};

	/// <summary>
	/// Wraps a data source so its JSON is upgraded before it reaches the deserializer.
	/// </summary>
	public static IJsonDataSource Wrap(IJsonDataSource source, MetadataJsonSerializerOptions options) => _instance.WrapSource(source, options, Constants.DefaultVersion);

	protected override int ReadSourceVersion(ReadOnlySequence<byte> source, MetadataJsonSerializerOptions options) => TryReadSettingsVersion(source);

	protected override bool ResolveEventType(ref Utf8JsonReader reader, out int key)
	{
		if (EnumConverter.TryParse(ref reader, out EventType type))
		{
			key = (int)type;
			return true;
		}
		key = 0;
		return false;
	}

	protected override void AddUpgraders(JsonUpgradePipeline pipeline)
	{
		// Object-level: normalizes SetGameSound to soundType + positional sounds.
		pipeline.Add(new SetGameSoundObjectUpgrader());
		// Field-level: rename the legacy event type name.
		AddField(pipeline, null, int.MaxValue, EventsPath, "type", value: UpgradeEventType);
		// Field-level: drop legacy fields that newer formats no longer emit.
		AddDrop(pipeline, EventType.ShowDialogue, "speed");
		AddDrop(pipeline, EventType.SetClapSounds, "p1Used");
		AddDrop(pipeline, EventType.SetClapSounds, "p2Used");
		AddDrop(pipeline, EventType.SetClapSounds, "cpuUsed");
		AddDrop(pipeline, EventType.FloatingText, "times");
		AddDrop(pipeline, EventType.NewWindowDance, "rooms");
		AddDrop(pipeline, EventType.MaskRoom, "rooms");
		AddDrop(pipeline, EventType.MaskRoom, "contentMode");
		AddDrop(pipeline, EventType.PlaySound, "isCustom");
		AddDrop(pipeline, null, "effectSound");
		// Field-level: value remaps.
		AddValue(pipeline, EventType.NarrateRowInfo, int.MaxValue, "narrateSkipBeats", StringMap(new Dictionary<string, string>
		{
			["on"] = "On",
			["off"] = "Off",
			["custom"] = "Custom",
		}));
		AddValue(pipeline, EventType.FloatingText, int.MaxValue, "narrationCategory", StringMap(new Dictionary<string, string>
		{
			["Main"] = "Fallback",
		}));
		AddValue(pipeline, EventType.ShakeScreen, int.MaxValue, "shakeLevel", UpgradeShakeLevel);
		AddValue(pipeline, EventType.SetTheme, 8, "preset", StringMap(new Dictionary<string, string>
		{
			["Kaleidoscope"] = "HallOfMirrors",
		}));
	}

	private static void AddField(
		JsonUpgradePipeline pipeline,
		EventType? key,
		int maxVersion,
		string pathPattern,
		string? field = null,
		JsonFieldNameFunc? rewrite = null,
		JsonFieldValueFunc? value = null) =>
		pipeline.Add(new FieldUpgrader(key is null ? null : (int)key.Value, maxVersion, pathPattern, field, rewrite, value));

	private static void AddDrop(
		JsonUpgradePipeline pipeline,
		EventType? key,
		string field,
		int maxVersion = int.MaxValue) =>
		pipeline.Add(new FieldUpgrader(key is null ? null : (int)key.Value, maxVersion, EventsPath, field, static (_, _, _) => FieldRewrite.Remove));

	private static void AddValue(
		JsonUpgradePipeline pipeline,
		EventType? key,
		int maxVersion,
		string field,
		JsonFieldValueFunc value) =>
		pipeline.Add(new FieldUpgrader(key is null ? null : (int)key.Value, maxVersion, EventsPath, field, value: value));

	private static JsonFieldValueFunc StringMap(IReadOnlyDictionary<string, string> map) =>
		(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonPath path, string fieldName, JsonUpgradeContext context) =>
		{
			if (reader.TokenType != JsonTokenType.String) return false;
			string? value = reader.GetString();
			writer.WriteStringValue(value is not null && map.TryGetValue(value, out string? mapped) ? mapped : value);
			return true;
		};

	private static bool UpgradeEventType(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonPath path, string fieldName, JsonUpgradeContext context)
	{
		if (reader.TokenType != JsonTokenType.String) return false;
		string? raw = reader.GetString();
		if (raw is null || EnumConverter.TryParse(ref reader, out EventType _))
			return false;
		if (EventTypeAliases.TryGetValue(raw, out string? canonical) && EnumConverter.TryParse(canonical, out EventType aliased))
		{
			writer.WriteStringValue(aliased.ToEnumUtf8String());
			return true;
		}
		return false;
	}

	private static bool UpgradeShakeLevel(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonPath path, string fieldName, JsonUpgradeContext context)
	{
		// Legacy shakeLevel values failed StrengthType parsing and were treated as the default (Low).
		if (reader.TokenType != JsonTokenType.String) return false;
		string? level = reader.GetString();
		writer.WriteStringValue(level is not null && EnumConverter.TryParse(level, out StrengthType _) ? level : nameof(StrengthType.Low));
		return true;
	}

	/// <summary>
	/// Reads <c>settings.version</c> without materializing the document so the upgrade pass can be
	/// version-gated before deserialization (which is what normally populates the version).
	/// </summary>
	private static int TryReadSettingsVersion(ReadOnlySequence<byte> source)
	{
		try
		{
			Utf8JsonReader reader = new(source, new JsonReaderOptions
			{
				CommentHandling = JsonCommentHandling.Skip,
				AllowTrailingCommas = true,
			});
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject) return 0;

			while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
			{
				if (reader.TokenType != JsonTokenType.PropertyName) break;

				if (reader.ValueTextEquals("settings"))
				{
					reader.Read();
					if (reader.TokenType != JsonTokenType.StartObject) return 0;
					while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
					{
						if (reader.TokenType != JsonTokenType.PropertyName) break;
						if (reader.ValueTextEquals("version"))
						{
							reader.Read();
							if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int number))
								return number;
							if (reader.TokenType == JsonTokenType.String && int.TryParse(reader.GetString(), out int parsed))
								return parsed;
							return 0;
						}
						reader.Read();
						reader.Skip();
					}
					return 0;
				}

				reader.Read();
				reader.Skip();
			}
		}
		catch (JsonException)
		{
			// Malformed settings.version simply disables version gating; the deserializer reports the real error.
		}
		return 0;
	}
}

/// <summary>
/// Object-level upgrader for <c>SetGameSound</c>. Migrates the legacy shapes (flat audio fields, or a
/// legacy <c>soundSubtypes</c> array) into the current shape: <c>soundType</c> plus a positional
/// <c>sounds</c> array whose entries follow the group members from <see cref="Constants.SoundGroupTypeMap"/>.
/// Objects that are already in the current shape are passed through unchanged.
/// </summary>
internal sealed class SetGameSoundObjectUpgrader : JsonObjectUpgrader
{
	private static readonly PathPattern EventPath = new("$.events[*]");

	public override int? Key => (int)EventType.SetGameSound;

	public override bool Matches(in JsonPath path, JsonUpgradeContext context) => EventPath.Matches(path);

	public override void Upgrade(JsonElement source, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		if (source.TryGetProperty("sounds", out _))
		{
			// Already in the current shape.
			foreach (JsonProperty property in source.EnumerateObject())
			{
				writer.WritePropertyName(property.Name);
				property.Value.WriteTo(writer);
			}
			return;
		}

		string? rawSoundType = source.TryGetProperty("soundType", out JsonElement typeElement) && typeElement.ValueKind == JsonValueKind.String
			? typeElement.GetString()
			: null;
		bool hasType = TryParseSoundType(rawSoundType, out SoundType soundType);

		SoundType[]? members = null;
		SoundType groupKey = default;
		if (hasType)
		{
			if (Constants.SoundGroupTypeMap.TryGetValue(soundType, out SoundType[]? groupMembers))
			{
				// soundType is a group key.
				members = groupMembers;
				groupKey = soundType;
			}
			else if (TryFindGroup(soundType, out groupKey, out SoundType[]? foundMembers))
			{
				// soundType is a member of a group.
				members = foundMembers;
			}
		}

		List<Slot> slots = BuildSlots(source, members, soundType, hasType);
		foreach (Slot slot in slots)
			ApplyVersionFixes(slot, context.SourceVersion);

		string outputSoundType = members is not null
			? groupKey.ToEnumString()
			: hasType ? soundType.ToEnumString() : rawSoundType ?? "";

		foreach (JsonProperty property in source.EnumerateObject())
		{
			switch (property.Name)
			{
				case "soundType":
				case "soundSubtypes":
				case "filename":
				case "volume":
				case "pitch":
				case "pan":
				case "offset":
					break;
				default:
					writer.WritePropertyName(property.Name);
					property.Value.WriteTo(writer);
					break;
			}
		}

		writer.WriteString("soundType", outputSoundType);
		writer.WritePropertyName("sounds");
		writer.WriteStartArray();
		foreach (Slot slot in slots)
			slot.Write(writer);
		writer.WriteEndArray();
	}

	private static List<Slot> BuildSlots(JsonElement source, SoundType[]? members, SoundType soundType, bool hasType)
	{
		if (source.TryGetProperty("soundSubtypes", out JsonElement array) && array.ValueKind == JsonValueKind.Array)
		{
			if (members is not null)
			{
				List<Slot> slots = EmptySlots(members.Length);
				int positional = 0;
				foreach (JsonElement element in array.EnumerateArray())
				{
					Slot slot = Slot.FromElement(element);
					int index = -1;
					if (element.ValueKind == JsonValueKind.Object &&
						element.TryGetProperty("groupSubtype", out JsonElement subtype) &&
						subtype.ValueKind == JsonValueKind.String &&
						EnumConverter.TryParse(subtype.GetString(), out SoundType parsed))
						index = Array.IndexOf(members, parsed);
					if (index < 0)
						index = positional;
					if ((uint)index < (uint)slots.Count)
						slots[index] = slot;
					positional++;
				}
				return slots;
			}

			// Single sound: keep the first entry.
			JsonElement first = array.GetArrayLength() > 0 ? array[0] : default;
			return [first.ValueKind == JsonValueKind.Object ? Slot.FromElement(first) : new Slot()];
		}

		Slot flat = Slot.FromFlat(source);
		if (members is not null && hasType)
		{
			int index = Array.IndexOf(members, soundType);
			if (index >= 0)
			{
				List<Slot> slots = EmptySlots(members.Length);
				slots[index] = flat;
				return slots;
			}
		}
		return [flat];
	}

	private static List<Slot> EmptySlots(int count)
	{
		List<Slot> slots = new(count);
		for (int i = 0; i < count; i++)
			slots.Add(new Slot { Used = false });
		return slots;
	}

	private static void ApplyVersionFixes(Slot slot, int version)
	{
		if (!slot.Used || slot.Filename is null)
			return;
		if (version <= 42)
		{
			if (slot.Filename == "Stick")
				slot.Filename = "StickOld";
			else if (slot.Filename == "ClosedHat")
				slot.Filename = "ClosedHatOld";
		}
		if (version <= 9)
			slot.Volume = (int)(slot.Volume / 0.4f);
	}

	private static bool TryParseSoundType(string? raw, out SoundType type)
	{
		type = default;
		if (string.IsNullOrEmpty(raw))
			return false;
		if (raw == "ClapSoundP1Hold")
			raw = nameof(SoundType.ClapSoundHold);
		return EnumConverter.TryParse(raw, out type);
	}

	private static bool TryFindGroup(SoundType type, out SoundType group, out SoundType[] members)
	{
		foreach (KeyValuePair<SoundType, SoundType[]> pair in Constants.SoundGroupTypeMap)
		{
			if (Array.IndexOf(pair.Value, type) >= 0)
			{
				group = pair.Key;
				members = pair.Value;
				return true;
			}
		}
		group = default;
		members = [];
		return false;
	}

	private sealed class Slot
	{
		public bool Used = true;
		public string? Filename;
		public int Volume = 100;
		public int Pitch = 100;
		public int Pan;
		public double OffsetMs;

		public static Slot FromElement(JsonElement element)
		{
			Slot slot = new();
			if (element.ValueKind != JsonValueKind.Object)
			{
				slot.Used = false;
				return slot;
			}
			if (element.TryGetProperty("used", out JsonElement used) && used.ValueKind == JsonValueKind.False)
				slot.Used = false;
			if (slot.Used)
				slot.ReadAudio(element);
			return slot;
		}

		public static Slot FromFlat(JsonElement source)
		{
			Slot slot = new();
			slot.ReadAudio(source);
			return slot;
		}

		public void Write(Utf8JsonWriter writer)
		{
			writer.WriteStartObject();
			if (!Used)
				writer.WriteBoolean("used", false);
			else
			{
				writer.WriteString("filename", Filename ?? "");
				if (Volume != 100) writer.WriteNumber("volume", Volume);
				if (Pitch != 100) writer.WriteNumber("pitch", Pitch);
				if (Pan != 0) writer.WriteNumber("pan", Pan);
				if (OffsetMs != 0) writer.WriteNumber("offset", OffsetMs);
			}
			writer.WriteEndObject();
		}

		private void ReadAudio(JsonElement element)
		{
			if (element.TryGetProperty("filename", out JsonElement filename) && filename.ValueKind == JsonValueKind.String)
				Filename = filename.GetString();
			if (element.TryGetProperty("volume", out JsonElement volume) && volume.ValueKind == JsonValueKind.Number && volume.TryGetInt32(out int v))
				Volume = v;
			if (element.TryGetProperty("pitch", out JsonElement pitch) && pitch.ValueKind == JsonValueKind.Number && pitch.TryGetInt32(out int p))
				Pitch = p;
			if (element.TryGetProperty("pan", out JsonElement pan) && pan.ValueKind == JsonValueKind.Number && pan.TryGetInt32(out int pa))
				Pan = pa;
			if (element.TryGetProperty("offset", out JsonElement offset) && offset.ValueKind == JsonValueKind.Number && offset.TryGetDouble(out double o))
				OffsetMs = o;
		}
	}
}
