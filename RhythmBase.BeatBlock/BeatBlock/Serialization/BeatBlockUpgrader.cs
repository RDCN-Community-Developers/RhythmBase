using RhythmBase.Global.Serialization;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Serialization;

/// <summary>
/// BeatBlock event upgraders and the entry point that inserts the upgrade pass before deserialization.
/// Upgraders are seed-keyed (event type + version): observers only update <see cref="BeatBlockUpgradeState"/>,
/// consumers rewrite using it, and emitters append generated events. Typed upgraders are registered before the
/// catch-all so the first match wins.
/// </summary>
internal sealed class BeatBlockUpgrader : BeatBlockUpgraderBase
{
	private const string EventsPath = "$.events[*]";
	private const string RootPath = "$[*]";

	private static readonly BeatBlockUpgrader _instance = new();

	/// <summary>
	/// Wraps a data source so its event JSON is upgraded before it reaches the deserializer.
	/// </summary>
	public static IJsonDataSource Wrap(IJsonDataSource source, MetadataJsonSerializerOptions options) => _instance.WrapSource(source, options, Constants.DefaultVersion);

	protected override JsonUpgradeState? CreateState() => new BeatBlockUpgradeState();

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
		// format 2: "beat" -> "block"
		Add(pipeline, "beat", 1, UpgradeBeat);

		// format 3 / 4: angle1 -> angle
		Add(pipeline, (int)EventType.Hold, 2, UpgradeAngle1);
		Add(pipeline, "minehold", 3, UpgradeAngle1);

		// observer: play records bpm (format 6) and time (format 13)
		Add(pipeline, (int)EventType.Play, 12, ObservePlay);

		// format 6 / 11: width duration in beats, then width -> paddles
		Add(pipeline, "width", 10, UpgradeWidth);

		// format 11: paddleCount -> one paddles event per paddle
		Add(pipeline, "paddleCount", 10, EmitPaddleCount);

		// format 10 / 12 / 15 / 17: deco order/drawOrder/default/effectCanvas(+Raw) and the effectCanvas boolean
		Add(pipeline, (int)EventType.Decoration, 16, UpgradeDecoration);

		// format 12: zero-duration setColor / ease get order -999
		Add(pipeline, (int)EventType.SetColor, 11, UpgradeOrderDefault);
		Add(pipeline, (int)EventType.Ease, 11, UpgradeOrderDefault);

		// format 13: setBPM shifted by the play time
		Add(pipeline, (int)EventType.SetBeatsPerMinute, 12, UpgradeSetBeatsPerMinute);

		// format 14: playSound gets ".ogg" when it is not a built-in sound name
		Add(pipeline, (int)EventType.PlaySound, 13, UpgradePlaySound);

		// format 16: paddles are stored in the level
		Add(pipeline, (int)EventType.Paddles, 15, UpgradePaddlesStore);

		// format 18: easeSequence -> appended setBoolean + comment (catch-all, registered last)
		Add(pipeline, (int?)null, 17, UpgradeEaseSequence);
	}

	private static void Add(JsonUpgradePipeline pipeline, int? key, int maxVersion, JsonEventUpgradeFunc upgrade)
	{
		pipeline.Add(new EventUpgrader(key, maxVersion, EventsPath, upgrade));
		pipeline.Add(new EventUpgrader(key, maxVersion, RootPath, upgrade));
	}

	private static void Add(JsonUpgradePipeline pipeline, string keyName, int maxVersion, JsonEventUpgradeFunc upgrade)
	{
		pipeline.Add(new EventUpgrader(keyName, maxVersion, EventsPath, upgrade));
		pipeline.Add(new EventUpgrader(keyName, maxVersion, RootPath, upgrade));
	}

	// ---- observers / consumers ---------------------------------------------------------------------

	private static void UpgradeBeat(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context) =>
		WriteTypeChanged(seed, writer, "block");

	private static void UpgradeAngle1(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context) =>
		WriteRenamed(seed, writer, "angle1", "angle");

	private static void UpgradePaddlesStore(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context) =>
		WriteFieldAdded(seed, writer, "forceStoreInLevel", true);

	private static void ObservePlay(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		if (context.State is BeatBlockUpgradeState state)
		{
			if (GetFloat(seed, "bpm") is float bpm)
				state.Bpm = bpm;
			if (GetFloat(seed, "time") is float time)
				state.PlaySongTime = time;
		}
		seed.WriteTo(writer);
	}

	private static void UpgradeWidth(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		Event e = Event.From(seed);
		if (context.SourceVersion < 6 && e.GetFloat("duration") is float duration && context.State is BeatBlockUpgradeState state)
			e.SetNumber("duration", duration / (3600f / state.Bpm));
		if (context.SourceVersion < 11)
		{
			e.SetString("type", "paddles");
			e.SetNumber("paddle", 0);
		}
		e.Write(writer);
	}

	private static void UpgradeDecoration(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		Event e = Event.From(seed);
		if (context.SourceVersion < 10 && e.Has("order"))
		{
			e.Set("drawOrder", e.Get("order"));
			e.Remove("order");
		}
		if (context.SourceVersion < 12 && e.IsZeroDuration())
			e.SetNumber("order", -999);
		bool effectCanvas = e.GetBool("effectCanvas");
		if (context.SourceVersion < 17 && effectCanvas)
			e.SetBool("effectCanvasRaw", true);
		e.Write(writer);

		if (context.SourceVersion < 15 && effectCanvas && context.State is BeatBlockUpgradeState state && !state.HasEffectCanvas)
		{
			state.HasEffectCanvas = true;
			state.EffectCanvasTime = e.GetFloat("time") ?? 0f;
			WriteSetBoolean(writer, state.EffectCanvasTime, -999, "vfx.effectCanvas.oldColors", true);
		}
	}

	private static void UpgradeOrderDefault(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		Event e = Event.From(seed);
		if (context.SourceVersion < 12 && e.IsZeroDuration())
			e.SetNumber("order", -999);
		e.Write(writer);
	}

	private static void UpgradeSetBeatsPerMinute(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		Event e = Event.From(seed);
		if (context.SourceVersion < 13
			&& context.State is BeatBlockUpgradeState state && state.PlaySongTime != 0f
			&& e.GetFloat("time") is float time)
			e.SetNumber("time", time + state.PlaySongTime);
		e.Write(writer);
	}

	private static void UpgradePlaySound(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		Event e = Event.From(seed);
		if (context.SourceVersion < 14 && e.GetString("sound") is string sound && !Constants.IsBuiltInSound(sound))
			e.SetString("sound", sound + ".ogg");
		e.Write(writer);
	}

	// ---- emitters ----------------------------------------------------------------------------------

	private static void EmitPaddleCount(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		Event e = Event.From(seed);
		float? count = e.GetFloat("paddles");
		e.SetString("type", "paddles");
		e.SetNumber("order", -999);
		e.SetNumber("paddle", 0);
		e.SetBool("enabled", false);
		e.SetNumber("duration", 0);
		e.Remove("paddles");
		e.Write(writer);

		if (count is not float paddleCount || paddleCount < 1)
			return;
		float distance = 360f / paddleCount;
		for (int i = 1; i <= (int)paddleCount; i++)
		{
			writer.WriteStartObject();
			writer.WriteString("type", "paddles");
			if (e.Has("angle")) { writer.WritePropertyName("angle"); e.Get("angle").WriteTo(writer); }
			if (e.Has("time")) { writer.WritePropertyName("time"); e.Get("time").WriteTo(writer); }
			writer.WriteNumber("order", 0);
			writer.WriteBoolean("enabled", true);
			writer.WriteNumber("duration", 0);
			writer.WriteNumber("paddle", i);
			writer.WriteNumber("newAngle", (i - 1) * distance);
			writer.WriteEndObject();
		}
	}

	private static void UpgradeEaseSequence(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context)
	{
		if (context.SourceVersion < 18
			&& IsTrue(seed, "easeSequence")
			&& context.State is BeatBlockUpgradeState state && !state.HasEaseSequence)
		{
			state.HasEaseSequence = true;
			state.EaseTime = GetFloat(seed, "time") ?? 0f;
			seed.WriteTo(writer);

			float time = state.EaseTime - 8f;
			WriteSetBoolean(writer, time, -1, "vfx.useVFXDistanceForVFXAngle", false);
			writer.WriteStartObject();
			writer.WriteString("type", "comment");
			writer.WriteNumber("angle", 10);
			writer.WriteNumber("time", time);
			writer.WriteString("text", "This boolean was added for backwards compatibility when this level was upgraded from format 17 to format 18.\nVersion 18: use VFX distance (from ease sequence) for VFX angle calculation\nIf the new behavior is wanted, simply delete the boolean and this comment.");
			writer.WriteEndObject();
			return;
		}
		seed.WriteTo(writer);
	}

	// ---- helpers -----------------------------------------------------------------------------------

	private static void WriteTypeChanged(JsonElement seed, Utf8JsonWriter writer, string newType)
	{
		Event e = Event.From(seed);
		e.SetString("type", newType);
		e.Write(writer);
	}

	private static void WriteRenamed(JsonElement seed, Utf8JsonWriter writer, string from, string to)
	{
		Event e = Event.From(seed);
		if (e.Has(from))
		{
			e.Set(to, e.Get(from));
			e.Remove(from);
		}
		e.Write(writer);
	}

	private static void WriteFieldAdded(JsonElement seed, Utf8JsonWriter writer, string field, bool value)
	{
		Event e = Event.From(seed);
		e.SetBool(field, value);
		e.Write(writer);
	}

	private static void WriteSetBoolean(Utf8JsonWriter writer, float time, int order, string var, bool enable)
	{
		writer.WriteStartObject();
		writer.WriteString("type", "setBoolean");
		writer.WriteNumber("angle", 0);
		writer.WriteNumber("time", time);
		writer.WriteNumber("order", order);
		writer.WriteString("var", var);
		writer.WriteBoolean("enable", enable);
		writer.WriteEndObject();
	}

	private static float? GetFloat(JsonElement element, string key) =>
		element.ValueKind == JsonValueKind.Object && element.TryGetProperty(key, out JsonElement v) && v.ValueKind == JsonValueKind.Number && v.TryGetSingle(out float f) ? f : null;

	private static bool IsTrue(JsonElement element, string key) =>
		element.ValueKind == JsonValueKind.Object && element.TryGetProperty(key, out JsonElement v) && v.ValueKind == JsonValueKind.True;

	/// <summary>A mutable JSON event object.</summary>
	private sealed class Event
	{
		private readonly Dictionary<string, JsonElement> _values;

		private Event(Dictionary<string, JsonElement> values) => _values = values;

		public static Event From(JsonElement source)
		{
			Dictionary<string, JsonElement> values = new(StringComparer.Ordinal);
			foreach (JsonProperty property in source.EnumerateObject())
				values[property.Name] = property.Value;
			return new Event(values);
		}

		public bool Has(string key) => _values.ContainsKey(key);
		public JsonElement Get(string key) => _values[key];
		public string? GetString(string key) => _values.TryGetValue(key, out JsonElement v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
		public float? GetFloat(string key) => _values.TryGetValue(key, out JsonElement v) && v.ValueKind == JsonValueKind.Number && v.TryGetSingle(out float f) ? f : null;
		public bool GetBool(string key) => _values.TryGetValue(key, out JsonElement v) && v.ValueKind == JsonValueKind.True;

		public void Set(string key, JsonElement value) => _values[key] = value;
		public void SetString(string key, string value) => _values[key] = JsonSerializer.SerializeToElement(value);
		public void SetNumber(string key, double value) => _values[key] = JsonSerializer.SerializeToElement(value);
		public void SetBool(string key, bool value) => _values[key] = JsonSerializer.SerializeToElement(value);
		public void Remove(string key) => _values.Remove(key);

		/// <summary>format 12: order is null/0 and duration is null/0.</summary>
		public bool IsZeroDuration()
		{
			float? order = GetFloat("order");
			float? duration = GetFloat("duration");
			return (order is null or 0) && (duration is null or 0);
		}

		public void Write(Utf8JsonWriter writer)
		{
			writer.WriteStartObject();
			foreach (KeyValuePair<string, JsonElement> pair in _values)
			{
				writer.WritePropertyName(pair.Key);
				pair.Value.WriteTo(writer);
			}
			writer.WriteEndObject();
		}
	}
}

/// <summary>
/// State tracked across events during a BeatBlock upgrade pass.
/// </summary>
internal sealed class BeatBlockUpgradeState : JsonUpgradeState
{
	/// <summary>The running BPM, taken from the last <c>play</c> event.</summary>
	public float Bpm = Constants.DefaultBpm;
	/// <summary>The time of the last <c>play</c> event (format 13).</summary>
	public float PlaySongTime;
	/// <summary>Whether an effectCanvas decoration has been seen (format 15).</summary>
	public bool HasEffectCanvas;
	/// <summary>The time of the first effectCanvas decoration (format 15).</summary>
	public float EffectCanvasTime;
	/// <summary>Whether an easeSequence event has been seen (format 18).</summary>
	public bool HasEaseSequence;
	/// <summary>The time of the first easeSequence event (format 18).</summary>
	public float EaseTime;
}
