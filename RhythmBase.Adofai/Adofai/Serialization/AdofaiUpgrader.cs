using RhythmBase.Global.Serialization;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace RhythmBase.Adofai.Serialization;

/// <summary>
/// Registry of the Adofai JSON upgraders and the entry point that inserts the upgrade pass
/// between an <see cref="IJsonDataSource"/> and deserialization.
/// </summary>
/// <remarks>
/// The game upgrades old levels inside <c>ADOFAI.LevelData.Decode</c>. This mirrors that logic:
/// per-version behaviour toggles, the legacy relative <c>pathData</c> to absolute <c>angleData</c>
/// migration, the version-7 <c>enabled</c> -> <c>active</c> rename, and version normalization so the
/// strict RhythmBase deserializer accepts the result.
/// </remarks>
internal sealed class AdofaiUpgrader : IBaseEventAdofaiUpgraderBase
{
	/// <summary>
	/// The version below which the pass runs. Aligned with the game's current format so every older
	/// document is normalized; documents already at <see cref="Constants.DefaultVersion"/> pass through.
	/// </summary>
	private const int TargetVersion = Constants.DefaultVersion;

	private static readonly AdofaiUpgrader _instance = new();

	/// <summary>
	/// Wraps a data source so its JSON is upgraded before it reaches the deserializer.
	/// </summary>
	public static IJsonDataSource Wrap(IJsonDataSource source, MetadataJsonSerializerOptions options)
		=> _instance.WrapSource(source, options, TargetVersion);

	protected override int ReadSourceVersion(ReadOnlySequence<byte> source, MetadataJsonSerializerOptions options)
		=> TryReadSettingsVersion(source);

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
		=> pipeline.Add(new AdofaiLevelUpgrader());

	/// <summary>
	/// Reads <c>settings.version</c> without materializing the document so the pass can be version-gated
	/// before deserialization.
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
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
				return 0;

			while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
			{
				if (reader.TokenType != JsonTokenType.PropertyName)
					break;

				if (reader.ValueTextEquals("settings"))
				{
					reader.Read();
					if (reader.TokenType != JsonTokenType.StartObject)
						return 0;
					while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
					{
						if (reader.TokenType != JsonTokenType.PropertyName)
							break;
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
			// Malformed settings.version disables version gating; the deserializer reports the real error.
		}
		return 0;
	}
}

/// <summary>
/// Buffered root upgrader mirroring <c>ADOFAI.LevelData.Decode</c>. It rewrites the whole document
/// because several migrations (pathData -> angleData, the version-7 rename) are not field-local.
/// </summary>
internal sealed class AdofaiLevelUpgrader : JsonObjectUpgrader
{
	private static readonly PathPattern RootPattern = new("$");

	public override bool Matches(in JsonPath path, JsonUpgradeContext context) => RootPattern.Matches(path);

	public override void Upgrade(JsonElement source, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		int version = ReadVersion(source);
		bool hasPathData = source.TryGetProperty("pathData", out JsonElement pathData) && pathData.ValueKind == JsonValueKind.String;
		JsonElement actions = source.TryGetProperty("actions", out JsonElement actionsElement) && actionsElement.ValueKind == JsonValueKind.Array
			? actionsElement
			: source.TryGetProperty("events", out JsonElement eventsElement) ? eventsElement : default;
		float[] tiles = ReadTiles(source, pathData, hasPathData);

		// ADOFAI.LevelData.Decode migrates floor/duration for formats 9..16.
		List<MutableEvent>? migrated = null;
		List<MutableEvent>? appended = null;
		if (version >= 9 && version <= 16 && tiles.Length > 0 && actions.ValueKind == JsonValueKind.Array)
			migrated = MigrateFloorEvents(tiles, actions, version, out appended);

		bool actionsWritten = false;
		foreach (JsonProperty property in source.EnumerateObject())
		{
			switch (property.Name)
			{
				case "settings":
					WriteSettings(writer, property.Value, version);
					break;

				case "pathData":
					WriteAngleData(writer, pathData);
					break;

				case "angleData":
					// When pathData is present it wins; the migrated array is written for "pathData".
					if (!hasPathData)
						WriteAngleDataValue(writer, property.Value);
					break;

				case "actions":
				case "events":
					if (actionsWritten)
						break; // both keys present; the actions array is emitted once
					actionsWritten = true;
					writer.WritePropertyName("actions");
					if (migrated is not null)
					{
						writer.WriteStartArray();
						foreach (MutableEvent entry in migrated)
							entry.Write(writer);
						if (appended is not null)
							foreach (MutableEvent entry in appended)
								entry.Write(writer);
						writer.WriteEndArray();
					}
					else
					{
						WriteValue(writer, property.Value, version);
					}
					break;

				default:
					writer.WritePropertyName(property.Name);
					WriteValue(writer, property.Value, version);
					break;
			}
		}
	}

	private const double TwoPi = Math.PI * 2;

	private static float[] ReadTiles(JsonElement source, JsonElement pathData, bool hasPathData)
	{
		if (hasPathData)
			return AdofaiPathData.Migrate(pathData.GetString() ?? "");
		if (source.TryGetProperty("angleData", out JsonElement angleData))
		{
			if (angleData.ValueKind == JsonValueKind.Array)
			{
				List<float> list = new(angleData.GetArrayLength());
				foreach (JsonElement item in angleData.EnumerateArray())
					list.Add(item.ValueKind == JsonValueKind.Number && item.TryGetSingle(out float angle) ? angle : 0f);
				return [.. list];
			}
			if (angleData.ValueKind == JsonValueKind.String && TryParseAngleString(angleData.GetString() ?? "", out float[] parsed))
				return parsed;
		}
		return [];
	}

	/// <summary>
	/// Port of <c>ADOFAI.LevelData.Decode</c> for formats 9..16: it corrects <c>Pause</c> durations using the
	/// floor turnaround and splits fractional <c>FreeRoam</c> durations into a <c>Pause</c> event.
	/// </summary>
	private static List<MutableEvent> MigrateFloorEvents(float[] tiles, JsonElement actions, int version, out List<MutableEvent> appended)
	{
		List<MutableEvent> events = [];
		foreach (JsonElement item in actions.EnumerateArray())
			events.Add(new MutableEvent(item));

		int count = tiles.Length;
		HashSet<int> twirls = [];
		Dictionary<int, int> planetsAt = [];
		Dictionary<int, int> holdAt = [];
		foreach (MutableEvent entry in events)
		{
			int index = (entry.GetInt("floor") ?? 0) - 1;
			switch (entry.EventType)
			{
				case "Twirl":
					if (!twirls.Add(index))
						twirls.Remove(index);
					break;
				case "MultiPlanet":
					if (entry.GetInt("planets") is int planets)
						planetsAt[index] = planets;
					break;
				case "Hold":
					int duration = entry.GetInt("duration") ?? 0;
					holdAt[index] = index < count ? duration : -1;
					break;
			}
		}

		// floor geometry; floor 0 is the start sentinel
		List<Floor> floors = [new Floor { Entry = 4.71238899230957, Exit = 0.0, Planets = 2, Hold = -1 }];
		bool ccw = false;
		int numPlanets = 2;
		int holdLength = -1;
		for (int i = 0; i < count; i++)
		{
			Floor current = floors[i];
			float angle = tiles[i];
			current.Exit = angle != 999f ? (90f - angle) * Math.PI / 180.0 : current.Entry;
			if (twirls.Contains(i))
				ccw = !ccw;
			if (planetsAt.TryGetValue(i, out int planets))
				numPlanets = planets;
			if (holdAt.TryGetValue(i, out int hold))
				holdLength = hold;
			if (angle == 999f)
				current.MidSpin = true;
			floors.Add(new Floor { Entry = Mod(current.Exit + Math.PI, TwoPi), Ccw = ccw, Planets = numPlanets, Hold = holdLength });
		}

		for (int i = 0; i < floors.Count; i++)
		{
			Floor floor = floors[i];
			Floor previous = floors[Math.Max(0, i - 1)];
			double offset = GetInverseAnglePerBeatMultiplanet(floor.Planets) * (floor.Ccw ? -1.0 : 1.0);
			if (floor.MidSpin)
				offset = 0.0;
			if (previous.MidSpin && floor.Planets > 2)
				offset -= (TwoPi + GetInverseAnglePerBeatMultiplanet(floor.Planets)) * (floor.Ccw ? -1.0 : 1.0);
			double moved = GetAngleMoved(floor.Entry + offset, floor.Exit + (floor.MidSpin ? offset : 0.0), !floor.Ccw);
			double magnitude = Math.Abs(moved);
			if (magnitude <= 1e-6 || magnitude >= 6.283184482025146)
			{
				if (floor.MidSpin)
					moved = 0.0;
				else
				{
					moved = TwoPi;
					floor.Turnaround = true;
				}
			}
			if (floor.Hold > 0)
				moved += floor.Hold * 2 * Math.PI;
			floor.AngleLength = moved;
		}

		appended = [];
		foreach (MutableEvent entry in events)
		{
			switch (entry.EventType)
			{
				case "Pause":
				{
					if (entry.GetFloat("duration") is not float duration)
						break;
					Floor floor = floors[Math.Min(entry.GetInt("floor") ?? 0, floors.Count - 1)];
					if (version == 9)
					{
						if (floor.Turnaround)
							entry.SetNumber("duration", duration + 1f);
						break;
					}
					bool fullTurn = Math.Abs(GetAngleMoved(floor.Entry, floor.Exit, !floor.Ccw) - TwoPi) < 0.0001;
					if (floor.Turnaround ^ fullTurn)
						entry.SetNumber("duration", Math.Max(0f, duration + (floor.Turnaround ? -1f : 1f)));
					break;
				}
				case "FreeRoam":
				{
					if (entry.GetFloat("duration") is not float duration)
						break;
					float fraction = duration % 1f;
					if (fraction != 0f)
					{
						MutableEvent? pause = events.FirstOrDefault(x => x.EventType == "Pause" && (x.GetInt("floor") ?? 0) == (entry.GetInt("floor") ?? 0));
						if (pause is not null)
						{
							if (pause.GetFloat("duration") is float pauseDuration)
								pause.SetNumber("duration", pauseDuration + fraction);
						}
						else
						{
							MutableEvent created = new();
							created.SetNumber("floor", entry.GetInt("floor") ?? 0);
							created.SetString("eventType", "Pause");
							created.SetNumber("duration", fraction);
							if (entry.Has("countdownTicks"))
								created.Set("countdownTicks", entry.Get("countdownTicks"));
							if (entry.Has("angleCorrectionDir"))
								created.Set("angleCorrectionDir", entry.Get("angleCorrectionDir"));
							appended.Add(created);
						}
					}
					entry.SetNumber("duration", Math.Max(0, Math.Round((double)duration - fraction)));
					break;
				}
			}
		}
		return events;
	}

	private static double Mod(double value, double modulus) => (value % modulus + modulus) % modulus;

	private static double GetAngleMoved(double entryAngle, double exitAngle, bool isCW)
		=> Mod((exitAngle - entryAngle) * (isCW ? 1.0 : -1.0), TwoPi);

	private static double GetInverseAnglePerBeatMultiplanet(double planets) => Math.PI * (planets - 2.0) / planets;

	private sealed class Floor
	{
		public double Entry;
		public double Exit;
		public double AngleLength;
		public bool Ccw;
		public bool MidSpin;
		public bool Turnaround;
		public int Planets;
		public int Hold;
	}

	/// <summary>An ordered, mutable JSON object (event) used during the floor migration.</summary>
	private sealed class MutableEvent
	{
		private readonly List<KeyValuePair<string, JsonElement>> _properties = [];

		public MutableEvent() { }

		public MutableEvent(JsonElement source)
		{
			foreach (JsonProperty property in source.EnumerateObject())
				_properties.Add(new KeyValuePair<string, JsonElement>(property.Name, property.Value));
		}

		public string EventType => GetString("eventType") ?? "";

		public bool Has(string name)
		{
			foreach (KeyValuePair<string, JsonElement> property in _properties)
				if (property.Key == name)
					return true;
			return false;
		}

		public JsonElement Get(string name)
		{
			foreach (KeyValuePair<string, JsonElement> property in _properties)
				if (property.Key == name)
					return property.Value;
			return default;
		}

		public string? GetString(string name) => Get(name) is { ValueKind: JsonValueKind.String } value ? value.GetString() : null;

		public int? GetInt(string name) => Get(name) is { ValueKind: JsonValueKind.Number } value && value.TryGetInt32(out int number) ? number : null;

		public float? GetFloat(string name) => Get(name) is { ValueKind: JsonValueKind.Number } value && value.TryGetSingle(out float number) ? number : null;

		public void Set(string name, JsonElement value)
		{
			for (int i = 0; i < _properties.Count; i++)
			{
				if (_properties[i].Key == name)
				{
					_properties[i] = new KeyValuePair<string, JsonElement>(name, value);
					return;
				}
			}
			_properties.Add(new KeyValuePair<string, JsonElement>(name, value));
		}

		public void SetString(string name, string value) => Set(name, JsonSerializer.SerializeToElement(value));

		public void SetNumber(string name, double value) => Set(name, JsonSerializer.SerializeToElement(value));

		public void Write(Utf8JsonWriter writer)
		{
			writer.WriteStartObject();
			foreach (KeyValuePair<string, JsonElement> property in _properties)
			{
				writer.WritePropertyName(property.Key);
				property.Value.WriteTo(writer);
			}
			writer.WriteEndObject();
		}
	}

	private static int ReadVersion(JsonElement source)
	{
		if (source.TryGetProperty("settings", out JsonElement settings) && settings.ValueKind == JsonValueKind.Object &&
			settings.TryGetProperty("version", out JsonElement version))
		{
			if (version.ValueKind == JsonValueKind.Number && version.TryGetInt32(out int number))
				return number < 1 ? 1 : number;
			if (version.ValueKind == JsonValueKind.String && int.TryParse(version.GetString(), out int parsed))
				return parsed < 1 ? 1 : parsed;
		}
		// The game defaults a missing version to 1.
		return 1;
	}

	private static void WriteAngleData(Utf8JsonWriter writer, JsonElement pathData)
	{
		string raw = pathData.GetString() ?? "";
		float[] angles = AdofaiPathData.Migrate(raw);
		writer.WritePropertyName("angleData");
		writer.WriteStartArray();
		foreach (float angle in angles)
			writer.WriteNumberValue(angle);
		writer.WriteEndArray();
	}

	private static void WriteAngleDataValue(Utf8JsonWriter writer, JsonElement value)
	{
		writer.WritePropertyName("angleData");
		if (value.ValueKind == JsonValueKind.String && TryParseAngleString(value.GetString() ?? "", out float[] parsed))
		{
			writer.WriteStartArray();
			foreach (float angle in parsed)
				writer.WriteNumberValue(angle);
			writer.WriteEndArray();
			return;
		}
		value.WriteTo(writer);
	}

	/// <summary>
	/// Some older documents stored tile angles as a string: either the legacy path characters or a
	/// delimiter-separated number list. Both are normalized into an absolute angle array.
	/// </summary>
	private static bool TryParseAngleString(string raw, out float[] angles)
	{
		angles = [];
		string trimmed = raw.Trim();
		if (trimmed.Length == 0)
			return false;

		bool pathLike = true;
		foreach (char c in trimmed)
		{
			if (!AdofaiPathData.IsPathChar(c))
			{
				pathLike = false;
				break;
			}
		}
		if (pathLike)
		{
			angles = AdofaiPathData.Migrate(trimmed);
			return true;
		}

		string[] parts = trimmed.Split([',', ';', ' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
		List<float> list = new(parts.Length);
		foreach (string part in parts)
		{
			if (!float.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
				return false;
			list.Add(parsed);
		}
		angles = [.. list];
		return angles.Length > 0;
	}

	private static void WriteSettings(Utf8JsonWriter writer, JsonElement settings, int version)
	{
		writer.WritePropertyName("settings");
		writer.WriteStartObject();
		if (settings.ValueKind == JsonValueKind.Object)
		{
			foreach (JsonProperty property in settings.EnumerateObject())
			{
				if (property.Name is "version" or "legacyFlash" or "legacySpriteTiles" or "legacyCamRelativeTo"
					or "legacyTween" or "disableV15Features" or "pitch")
					continue;
				writer.WritePropertyName(property.Name);
				property.Value.WriteTo(writer);
			}
		}

		// Normalize to at least the minimum version RhythmBase understands.
		writer.WriteNumber("version", Math.Max(version, Constants.DefaultVersion));

		// ADOFAI.LevelData.Decode: per-version legacy toggles.
		writer.WriteBoolean("legacyFlash", version < 4 || ReadBool(settings, "legacyFlash"));
		writer.WriteBoolean("legacySpriteTiles", version < 5 || ReadBool(settings, "legacySpriteTiles"));
		writer.WriteBoolean("legacyCamRelativeTo", version < 11 || ReadBool(settings, "legacyCamRelativeTo"));
		writer.WriteBoolean("legacyTween", version < 14 || ReadBool(settings, "legacyTween"));
		writer.WriteBoolean("disableV15Features", version < 15 || ReadBool(settings, "disableV15Features"));

		// Pitch was introduced in version 2; older documents used the default.
		if (version < 2)
			writer.WriteNumber("pitch", ReadInt(settings, "pitch") ?? 100);

		writer.WriteEndObject();
	}

	private static bool ReadBool(JsonElement settings, string name)
		=> settings.ValueKind == JsonValueKind.Object && settings.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.True;

	private static int? ReadInt(JsonElement settings, string name)
		=> settings.ValueKind == JsonValueKind.Object && settings.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int number) ? number : null;

	/// <summary>Recursively copies a value, applying the version-7 <c>enabled</c> -> <c>active</c> rename.</summary>
	private static void WriteValue(Utf8JsonWriter writer, JsonElement element, int version)
	{
		switch (element.ValueKind)
		{
			case JsonValueKind.Object:
				writer.WriteStartObject();
				foreach (JsonProperty property in element.EnumerateObject())
				{
					string name = version == 7 && property.Name == "enabled" ? "active" : property.Name;
					writer.WritePropertyName(name);
					WriteValue(writer, property.Value, version);
				}
				writer.WriteEndObject();
				break;

			case JsonValueKind.Array:
				writer.WriteStartArray();
				foreach (JsonElement item in element.EnumerateArray())
					WriteValue(writer, item, version);
				writer.WriteEndArray();
				break;

			default:
				element.WriteTo(writer);
				break;
		}
	}
}
