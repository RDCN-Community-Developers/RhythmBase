using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Global.Serialization;

/// <summary>
/// A single segment of a JSON path: either a property name or an array index.
/// </summary>
public readonly struct PathSegment
{
	public readonly string? Property;
	public readonly int Index;

	public PathSegment(string property) { Property = property; Index = -1; }
	public PathSegment(int index) { Property = null; Index = index; }

	public bool IsIndex => Property is null;
	public override string ToString() => IsIndex ? $"[{Index}]" : Property!;
}

/// <summary>
/// An immutable JSON path, used both for matching and for passing context to upgraders.
/// </summary>
public readonly struct JsonPath
{
	private readonly PathSegment[] _segments;
	private readonly int _count;

	internal JsonPath(PathSegment[] segments, int count)
	{
		_segments = segments;
		_count = count;
	}

	public int Count => _count;
	public PathSegment this[int i] => _segments[i];

	public override string ToString()
	{
		StringBuilder sb = new("$");
		for (int i = 0; i < _count; i++)
		{
			PathSegment s = _segments[i];
			if (s.IsIndex) sb.Append('[').Append(s.Index).Append(']');
			else sb.Append('.').Append(s.Property);
		}
		return sb.ToString();
	}
}

/// <summary>
/// A growable stack of <see cref="JsonPath"/> segments used by the pipeline during a single pass.
/// </summary>
public sealed class PathStack
{
	private PathSegment[] _segments = new PathSegment[16];
	private int _count;

	public JsonPath Current => new(_segments, _count);

	public void PushProperty(string name)
	{
		EnsureCapacity();
		_segments[_count++] = new PathSegment(name);
	}

	public void PushIndex(int index)
	{
		EnsureCapacity();
		_segments[_count++] = new PathSegment(index);
	}

	public void Pop() => _count--;
	public void Clear() => _count = 0;

	private void EnsureCapacity()
	{
		if (_count == _segments.Length)
			Array.Resize(ref _segments, _segments.Length * 2);
	}
}

/// <summary>
/// A compiled JSON path pattern supporting <c>*</c> wildcards, for example <c>$.events[*].ease</c>.
/// </summary>
public readonly struct PathPattern
{
	private readonly Segment[] _segments;

	private readonly struct Segment
	{
		public readonly string? Property;
		public readonly int Index;
		public readonly bool IsWildcard;
		public readonly bool IsIndex;

		public Segment(string? property, int index, bool isWildcard, bool isIndex)
		{
			Property = property;
			Index = index;
			IsWildcard = isWildcard;
			IsIndex = isIndex;
		}
	}

	public PathPattern(string pattern)
	{
		if (string.IsNullOrEmpty(pattern) || pattern[0] != '$')
			throw new ArgumentException("Pattern must start with '$'.", nameof(pattern));

		List<Segment> list = [];
		int i = 1;
		while (i < pattern.Length)
		{
			char c = pattern[i];
			if (c == '.')
			{
				i++;
				int start = i;
				while (i < pattern.Length && pattern[i] != '.' && pattern[i] != '[') i++;
				string name = pattern[start..i];
				list.Add(name == "*" ? new Segment(null, -1, true, false) : new Segment(name, -1, false, false));
			}
			else if (c == '[')
			{
				int end = pattern.IndexOf(']', i);
				if (end < 0) throw new ArgumentException("Unterminated '[' in pattern.", nameof(pattern));
				string inner = pattern.Substring(i + 1, end - i - 1);
				if (inner == "*") list.Add(new Segment(null, -1, true, true));
				else if (int.TryParse(inner, out int idx)) list.Add(new Segment(null, idx, false, true));
				else throw new ArgumentException($"Invalid array index '{inner}'.", nameof(pattern));
				i = end + 1;
			}
			else
			{
				throw new ArgumentException($"Unexpected char '{c}' at {i}.", nameof(pattern));
			}
		}
		_segments = list.ToArray();
	}

	public bool Matches(in JsonPath path)
	{
		if (path.Count != _segments.Length) return false;
		for (int i = 0; i < _segments.Length; i++)
		{
			Segment pat = _segments[i];
			PathSegment seg = path[i];

			if (pat.IsWildcard) continue;

			if (pat.IsIndex)
			{
				if (!seg.IsIndex || seg.Index != pat.Index) return false;
			}
			else
			{
				if (seg.IsIndex || !string.Equals(seg.Property, pat.Property, StringComparison.Ordinal)) return false;
			}
		}
		return true;
	}
}

/// <summary>
/// Base type for sub-project-specific state preserved while an upgrade pass (or a deserialization) runs,
/// for example a running BPM captured by an earlier event and consumed by a later one.
/// </summary>
public abstract class JsonUpgradeState { }

/// <summary>
/// Carries the source/target version, the sub-project state, and per-object discriminator state shared by
/// all upgraders of a pass.
/// </summary>
public sealed class JsonUpgradeContext
{
	public int SourceVersion { get; init; }
	public int TargetVersion { get; init; }

	/// <summary>
	/// Gets the sub-project-specific state carried through this pass, or <see langword="null"/>.
	/// </summary>
	public JsonUpgradeState? State { get; init; }

	private readonly List<(int? Key, string? Name)> _discriminators = [];

	/// <summary>
	/// Gets the enum discriminator key of the innermost enclosing object under a discriminator path
	/// (for example the event type of the current element of <c>$.events[*]</c>), or <see langword="null"/>
	/// when the type name did not resolve to an enum member.
	/// </summary>
	public int? CurrentDiscriminator => _discriminators.Count > 0 ? _discriminators[^1].Key : null;

	/// <summary>
	/// Gets the raw discriminator name of the innermost enclosing object under a discriminator path, or
	/// <see langword="null"/>. It is only set when the name did not resolve to an enum member, so upgrader
	/// registrations can fall back to string matching.
	/// </summary>
	public string? CurrentDiscriminatorName => _discriminators.Count > 0 ? _discriminators[^1].Name : null;

	internal void PushDiscriminator(int? key, string? name) => _discriminators.Add((key, name));
	internal void PopDiscriminator()
	{
		if (_discriminators.Count > 0) _discriminators.RemoveAt(_discriminators.Count - 1);
	}
}

/// <summary>
/// The result of a field-level rewrite: keep, drop, or rename the property.
/// </summary>
public readonly struct FieldRewrite
{
	public readonly bool Drop;
	public readonly string? Name;

	private FieldRewrite(bool drop, string? name)
	{
		Drop = drop;
		Name = name;
	}

	public static readonly FieldRewrite Keep = new(false, null);
	public static readonly FieldRewrite Remove = new(true, null);
	public static FieldRewrite Rename(string name) => new(false, name);
}

/// <summary>Decides how a matching property is handled.</summary>
public delegate FieldRewrite JsonFieldNameFunc(JsonPath path, string fieldName, JsonUpgradeContext context);

/// <summary>Rewrites a matching property value in place, returning <see langword="true"/> when it wrote the value.</summary>
public delegate bool JsonFieldValueFunc(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonPath path, string fieldName, JsonUpgradeContext context);

/// <summary>
/// Resolves the discriminator string currently read by <paramref name="reader"/> to an enum key without
/// allocating. Return <see langword="false"/> when the name has no enum member; the pipeline then keeps the raw
/// string so registrations can match it by name.
/// </summary>
public delegate bool JsonDiscriminatorResolver(ref Utf8JsonReader reader, out int key);

/// <summary>
/// A streaming, field-level upgrader. It is consulted for every object property and can rename it,
/// remove it, or rewrite its value in place, without buffering the owning object.
/// </summary>
public abstract class JsonFieldUpgrader
{
	/// <summary>
	/// The enum discriminator key this upgrader targets, or <see langword="null"/> to match by
	/// <see cref="KeyName"/> or to apply to every type. Filtered by the pipeline before <see cref="Matches"/>.
	/// </summary>
	public virtual int? Key => null;
	/// <summary>
	/// The raw discriminator name used when <see cref="Key"/> is <see langword="null"/>, so legacy type names
	/// without an enum member can be matched. <see langword="null"/> applies to every type.
	/// </summary>
	public virtual string? KeyName => null;
	/// <summary>
	/// The highest source version this upgrader applies to (inclusive). Defaults to every version.
	/// </summary>
	public virtual int MaxVersion => int.MaxValue;

	/// <summary>
	/// Determines whether this upgrader applies. <paramref name="path"/> is the path of the object that
	/// contains the property, not the property's own path.
	/// </summary>
	public abstract bool Matches(in JsonPath path, string fieldName, JsonUpgradeContext context);
	/// <summary>Decides the property's final name, or that it is removed.</summary>
	public abstract FieldRewrite Rewrite(in JsonPath path, string fieldName, JsonUpgradeContext context);
	/// <summary>
	/// Called with the reader positioned at the property value. Return <see langword="true"/> if the value
	/// was consumed and written; return <see langword="false"/> to let the pipeline copy it recursively.
	/// </summary>
	public virtual bool TryUpgradeValue(ref Utf8JsonReader reader, Utf8JsonWriter writer, in JsonPath path, string fieldName, JsonUpgradeContext context) => false;
}

/// <summary>
/// A buffered, object-level upgrader. It receives the whole object as a <see cref="JsonElement"/> so it can
/// combine, reorder, add or remove related fields. The pipeline writes the object braces; this upgrader writes
/// the properties. Field-level upgraders are not applied inside an object handled here.
/// </summary>
public abstract class JsonObjectUpgrader
{
	/// <summary>
	/// The enum discriminator key this upgrader targets, or <see langword="null"/> to match by
	/// <see cref="KeyName"/> or to apply to every type. Filtered by the pipeline.
	/// </summary>
	public virtual int? Key => null;
	/// <summary>
	/// The raw discriminator name used when <see cref="Key"/> is <see langword="null"/>, so legacy type names
	/// without an enum member can be matched. <see langword="null"/> applies to every type.
	/// </summary>
	public virtual string? KeyName => null;
	/// <summary>
	/// The highest source version this upgrader applies to (inclusive). Defaults to every version.
	/// </summary>
	public virtual int MaxVersion => int.MaxValue;

	/// <summary>Determines whether this upgrader applies to the given object.</summary>
	public abstract bool Matches(in JsonPath path, JsonUpgradeContext context);
	/// <summary>Writes the upgraded object's properties. The object braces are written by the pipeline.</summary>
	public abstract void Upgrade(JsonElement source, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context);
}

/// <summary>
/// A buffered, array-level (multi-object) upgrader. It receives the whole array as a <see cref="JsonElement"/>
/// so it can reorder, remove, insert or expand elements. The pipeline writes the array brackets; this upgrader
/// writes the elements. Use it for event-sequence upgrades, such as expanding one event into many.
/// </summary>
public abstract class JsonArrayUpgrader
{
	/// <summary>
	/// The enum discriminator key this upgrader targets, or <see langword="null"/> to match by
	/// <see cref="KeyName"/> or to apply to every type. Filtered by the pipeline.
	/// </summary>
	public virtual int? Key => null;
	/// <summary>
	/// The raw discriminator name used when <see cref="Key"/> is <see langword="null"/>, so legacy type names
	/// without an enum member can be matched. <see langword="null"/> applies to every type.
	/// </summary>
	public virtual string? KeyName => null;
	/// <summary>
	/// The highest source version this upgrader applies to (inclusive). Defaults to every version.
	/// </summary>
	public virtual int MaxVersion => int.MaxValue;

	/// <summary>Determines whether this upgrader applies to the given array.</summary>
	public abstract bool Matches(in JsonPath path, JsonUpgradeContext context);
	/// <summary>Writes the upgraded array's elements. The array brackets are written by the pipeline.</summary>
	public abstract void Upgrade(JsonElement source, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context);
}

/// <summary>
/// A per-event upgrader triggered by a seed event (its <see cref="Key"/>/<see cref="KeyName"/>). It receives the
/// seed event and writes one or more events with the writer, so it can rename fields, consume cross-event state
/// and append generated events. Register typed upgraders before catch-all ones: the first match wins.
/// </summary>
public abstract class JsonEventUpgrader
{
	/// <summary>
	/// The enum discriminator key of the seed event, or <see langword="null"/> to match by <see cref="KeyName"/>
	/// or to apply to every event.
	/// </summary>
	public virtual int? Key => null;
	/// <summary>
	/// The raw seed type name used when <see cref="Key"/> is <see langword="null"/>, so legacy type names without
	/// an enum member can be matched. <see langword="null"/> applies to every event.
	/// </summary>
	public virtual string? KeyName => null;
	/// <summary>
	/// The highest source version this upgrader applies to (inclusive). Defaults to every version.
	/// </summary>
	public virtual int MaxVersion => int.MaxValue;

	/// <summary>Determines whether this upgrader applies to the seed event's path.</summary>
	public abstract bool Matches(in JsonPath path, JsonUpgradeContext context);
	/// <summary>Writes the upgraded seed (and optionally appended) events.</summary>
	public abstract void Upgrade(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context);
}

/// <summary>Writes the upgraded seed and any appended events.</summary>
public delegate void JsonEventUpgradeFunc(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context);

/// <summary>
/// The single concrete event upgrader: a (enum key | name key, version, path) selector plus an upgrade delegate.
/// </summary>
public sealed class EventUpgrader : JsonEventUpgrader
{
	private readonly int? _key;
	private readonly string? _keyName;
	private readonly int _maxVersion;
	private readonly PathPattern _path;
	private readonly JsonEventUpgradeFunc _upgrade;

	/// <summary>Creates an event upgrader keyed by an enum discriminator.</summary>
	public EventUpgrader(int? key, int maxVersion, string pathPattern, JsonEventUpgradeFunc upgrade)
	{
		_key = key;
		_maxVersion = maxVersion;
		_path = new PathPattern(pathPattern);
		_upgrade = upgrade;
	}

	/// <summary>Creates an event upgrader keyed by a raw discriminator name (for legacy types).</summary>
	public EventUpgrader(string keyName, int maxVersion, string pathPattern, JsonEventUpgradeFunc upgrade)
	{
		_keyName = keyName;
		_maxVersion = maxVersion;
		_path = new PathPattern(pathPattern);
		_upgrade = upgrade;
	}

	public override int? Key => _key;
	public override string? KeyName => _keyName;
	public override int MaxVersion => _maxVersion;
	public override bool Matches(in JsonPath path, JsonUpgradeContext context) => _path.Matches(path);
	public override void Upgrade(JsonElement seed, Utf8JsonWriter writer, in JsonPath path, JsonUpgradeContext context) => _upgrade(seed, writer, path, context);
}

/// <summary>
/// The single concrete field upgrader: a (type, version, path, field) selector plus optional rewrite/value
/// delegates. Combine it for renames, removals and value remaps instead of deriving a type per case.
/// </summary>
public sealed class FieldUpgrader : JsonFieldUpgrader
{
	private readonly int? _key;
	private readonly string? _keyName;
	private readonly int _maxVersion;
	private readonly PathPattern _path;
	private readonly string? _field;
	private readonly JsonFieldNameFunc? _rewrite;
	private readonly JsonFieldValueFunc? _value;

	/// <summary>Creates a field upgrader keyed by an enum discriminator.</summary>
	public FieldUpgrader(
		int? key,
		int maxVersion,
		string pathPattern,
		string? field = null,
		JsonFieldNameFunc? rewrite = null,
		JsonFieldValueFunc? value = null)
	{
		_key = key;
		_maxVersion = maxVersion;
		_path = new PathPattern(pathPattern);
		_field = field;
		_rewrite = rewrite;
		_value = value;
	}

	/// <summary>Creates a field upgrader keyed by a raw discriminator name (for legacy types without an enum member).</summary>
	public FieldUpgrader(
		string keyName,
		int maxVersion,
		string pathPattern,
		string? field = null,
		JsonFieldNameFunc? rewrite = null,
		JsonFieldValueFunc? value = null)
	{
		_keyName = keyName;
		_maxVersion = maxVersion;
		_path = new PathPattern(pathPattern);
		_field = field;
		_rewrite = rewrite;
		_value = value;
	}

	public override int? Key => _key;
	public override string? KeyName => _keyName;
	public override int MaxVersion => _maxVersion;

	public override bool Matches(in JsonPath path, string fieldName, JsonUpgradeContext context) =>
		(_field is null || string.Equals(fieldName, _field, StringComparison.Ordinal)) && _path.Matches(path);

	public override FieldRewrite Rewrite(in JsonPath path, string fieldName, JsonUpgradeContext context) =>
		_rewrite?.Invoke(path, fieldName, context) ?? FieldRewrite.Keep;

	public override bool TryUpgradeValue(ref Utf8JsonReader reader, Utf8JsonWriter writer, in JsonPath path, string fieldName, JsonUpgradeContext context) =>
		_value?.Invoke(ref reader, writer, path, fieldName, context) ?? false;
}

/// <summary>
/// A streaming JSON upgrader. Field-level upgraders are applied in place while the document is copied;
/// object-level upgraders buffer only the objects they match. Objects under a registered discriminator path
/// have their discriminator resolved first so upgraders can be type-specific.
/// </summary>
public sealed class JsonUpgradePipeline
{
	private readonly struct DiscriminatorRule(PathPattern path, string field, JsonDiscriminatorResolver resolve)
	{
		public readonly PathPattern Path = path;
		public readonly string Field = field;
		public readonly JsonDiscriminatorResolver Resolve = resolve;
	}

	private readonly List<JsonFieldUpgrader> _fieldUpgraders = [];
	private readonly List<JsonObjectUpgrader> _objectUpgraders = [];
	private readonly List<JsonArrayUpgrader> _arrayUpgraders = [];
	private readonly List<JsonEventUpgrader> _eventUpgraders = [];
	private readonly List<DiscriminatorRule> _discriminators = [];
	private readonly PathStack _path = new();

	private static readonly JsonReaderOptions ReaderOptions = new()
	{
		CommentHandling = JsonCommentHandling.Skip,
		AllowTrailingCommas = true,
	};

	public JsonUpgradePipeline Add(JsonFieldUpgrader upgrader)
	{
		_fieldUpgraders.Add(upgrader);
		return this;
	}

	public JsonUpgradePipeline Add(JsonObjectUpgrader upgrader)
	{
		_objectUpgraders.Add(upgrader);
		return this;
	}

	public JsonUpgradePipeline Add(JsonArrayUpgrader upgrader)
	{
		_arrayUpgraders.Add(upgrader);
		return this;
	}

	public JsonUpgradePipeline Add(JsonEventUpgrader upgrader)
	{
		_eventUpgraders.Add(upgrader);
		return this;
	}

	/// <summary>
	/// Registers a path whose objects carry a discriminator (for example <c>$.events[*]</c> with field
	/// <c>type</c>). <paramref name="resolve"/> maps the raw discriminator string to the key exposed through
	/// <see cref="JsonUpgradeContext.CurrentDiscriminator"/>.
	/// </summary>
	public JsonUpgradePipeline AddDiscriminator(string pathPattern, string fieldName, JsonDiscriminatorResolver resolve)
	{
		_discriminators.Add(new DiscriminatorRule(new PathPattern(pathPattern), fieldName, resolve));
		return this;
	}

	/// <summary>Runs the upgrade pass over a (possibly multi-segment) UTF-8 buffer.</summary>
	public void Process(ReadOnlySequence<byte> utf8Json, Utf8JsonWriter writer, JsonUpgradeContext context)
	{
		Utf8JsonReader reader = new(utf8Json, ReaderOptions);
		if (!reader.Read()) return;

		_path.Clear();
		ProcessValue(ref reader, writer, context);
	}

	private void ProcessValue(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonUpgradeContext context)
	{
		switch (reader.TokenType)
		{
			case JsonTokenType.StartObject:
				WriteObject(ref reader, writer, context);
				break;

			case JsonTokenType.StartArray:
				WriteArray(ref reader, writer, context);
				break;

			default:
				CopyPrimitive(ref reader, writer);
				break;
		}
	}

	private void WriteObject(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonUpgradeContext context)
	{
		(int? Key, string? Name) discriminator = ResolveDiscriminator(ref reader);
		context.PushDiscriminator(discriminator.Key, discriminator.Name);
		try
		{
			JsonObjectUpgrader? objectUpgrader = FindObjectUpgrader(context);
			writer.WriteStartObject();
			if (objectUpgrader is null)
			{
				StreamObject(ref reader, writer, context);
			}
			else
			{
				using JsonDocument document = JsonDocument.ParseValue(ref reader);
				objectUpgrader.Upgrade(document.RootElement, writer, _path.Current, context);
			}
			writer.WriteEndObject();
		}
		finally
		{
			context.PopDiscriminator();
		}
	}

	private void StreamObject(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonUpgradeContext context)
	{
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			string originalName = reader.GetString()!;
			JsonPath parentPath = _path.Current;

			JsonFieldUpgrader? upgrader = FindFieldUpgrader(parentPath, originalName, context);
			FieldRewrite rewrite = upgrader?.Rewrite(parentPath, originalName, context) ?? FieldRewrite.Keep;

			if (rewrite.Drop)
			{
				reader.Read();
				reader.Skip();
				continue;
			}

			writer.WritePropertyName(rewrite.Name ?? originalName);

			reader.Read();
			if (upgrader is not null && upgrader.TryUpgradeValue(ref reader, writer, parentPath, originalName, context))
			{
				// The upgrader consumed and wrote the value.
			}
			else
			{
				_path.PushProperty(originalName);
				ProcessValue(ref reader, writer, context);
				_path.Pop();
			}
		}
	}

	private void WriteArray(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonUpgradeContext context)
	{
		JsonArrayUpgrader? arrayUpgrader = FindArrayUpgrader(context);
		writer.WriteStartArray();
		if (arrayUpgrader is null)
		{
			CopyArray(ref reader, writer, context);
		}
		else
		{
			using JsonDocument document = JsonDocument.ParseValue(ref reader);
			arrayUpgrader.Upgrade(document.RootElement, writer, _path.Current, context);
		}
		writer.WriteEndArray();
	}

	private void CopyArray(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonUpgradeContext context)
	{
		int index = 0;
		while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
		{
			_path.PushIndex(index++);
			if (_eventUpgraders.Count > 0 && reader.TokenType == JsonTokenType.StartObject && TryUpgradeEvent(ref reader, writer, context))
			{
				// Handled by an event upgrader.
			}
			else
			{
				ProcessValue(ref reader, writer, context);
			}
			_path.Pop();
		}
	}

	private bool TryUpgradeEvent(ref Utf8JsonReader reader, Utf8JsonWriter writer, JsonUpgradeContext context)
	{
		(int? Key, string? Name) discriminator = ResolveDiscriminator(ref reader);
		JsonEventUpgrader? upgrader = FindEventUpgrader(discriminator.Key, discriminator.Name, context);
		if (upgrader is null)
			return false;
		using JsonDocument document = JsonDocument.ParseValue(ref reader);
		upgrader.Upgrade(document.RootElement, writer, _path.Current, context);
		return true;
	}

	private static bool DiscriminatorMatches(int? key, string? keyName, int? elementKey, string? elementName)
	{
		if (key is int enumKey)
			return elementKey == enumKey;
		if (keyName is not null)
			return string.Equals(elementName, keyName, StringComparison.Ordinal);
		return true;
	}

	private JsonFieldUpgrader? FindFieldUpgrader(JsonPath path, string fieldName, JsonUpgradeContext context)
	{
		for (int i = 0; i < _fieldUpgraders.Count; i++)
		{
			JsonFieldUpgrader upgrader = _fieldUpgraders[i];
			if (context.SourceVersion <= upgrader.MaxVersion
				&& DiscriminatorMatches(upgrader.Key, upgrader.KeyName, context.CurrentDiscriminator, context.CurrentDiscriminatorName)
				&& upgrader.Matches(path, fieldName, context))
				return upgrader;
		}
		return null;
	}

	private JsonObjectUpgrader? FindObjectUpgrader(JsonUpgradeContext context)
	{
		JsonPath path = _path.Current;
		for (int i = 0; i < _objectUpgraders.Count; i++)
		{
			JsonObjectUpgrader upgrader = _objectUpgraders[i];
			if (context.SourceVersion <= upgrader.MaxVersion
				&& DiscriminatorMatches(upgrader.Key, upgrader.KeyName, context.CurrentDiscriminator, context.CurrentDiscriminatorName)
				&& upgrader.Matches(path, context))
				return upgrader;
		}
		return null;
	}

	private JsonArrayUpgrader? FindArrayUpgrader(JsonUpgradeContext context)
	{
		JsonPath path = _path.Current;
		for (int i = 0; i < _arrayUpgraders.Count; i++)
		{
			JsonArrayUpgrader upgrader = _arrayUpgraders[i];
			if (context.SourceVersion <= upgrader.MaxVersion
				&& DiscriminatorMatches(upgrader.Key, upgrader.KeyName, context.CurrentDiscriminator, context.CurrentDiscriminatorName)
				&& upgrader.Matches(path, context))
				return upgrader;
		}
		return null;
	}

	private JsonEventUpgrader? FindEventUpgrader(int? elementKey, string? elementName, JsonUpgradeContext context)
	{
		JsonPath path = _path.Current;
		for (int i = 0; i < _eventUpgraders.Count; i++)
		{
			JsonEventUpgrader upgrader = _eventUpgraders[i];
			if (context.SourceVersion <= upgrader.MaxVersion
				&& DiscriminatorMatches(upgrader.Key, upgrader.KeyName, elementKey, elementName)
				&& upgrader.Matches(path, context))
				return upgrader;
		}
		return null;
	}

	private (int? Key, string? Name) ResolveDiscriminator(ref Utf8JsonReader reader)
	{
		JsonPath path = _path.Current;
		for (int i = 0; i < _discriminators.Count; i++)
		{
			DiscriminatorRule rule = _discriminators[i];
			if (!rule.Path.Matches(path)) continue;

			Utf8JsonReader head = reader;
			(int? Key, string? Name) value = ScanDiscriminator(ref reader, rule);
			reader = head;
			return value;
		}
		return (null, null);
	}

	private static (int? Key, string? Name) ScanDiscriminator(ref Utf8JsonReader reader, DiscriminatorRule rule)
	{
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			if (reader.TokenType != JsonTokenType.PropertyName)
				break;

			if (reader.ValueTextEquals(rule.Field))
			{
				reader.Read();
				if (reader.TokenType != JsonTokenType.String) return (null, null);
				// Prefer the enum key; keep the raw name only when it does not resolve, for string matching.
				if (rule.Resolve(ref reader, out int key)) return (key, null);
				return (null, reader.GetString());
			}

			reader.Read();
			reader.Skip();
		}
		return (null, null);
	}

	private static void CopyPrimitive(ref Utf8JsonReader reader, Utf8JsonWriter writer)
	{
		switch (reader.TokenType)
		{
			case JsonTokenType.String:
				// GetString handles both multi-segment values and unescaping; the writer re-escapes.
				writer.WriteStringValue(reader.GetString());
				break;
			case JsonTokenType.Number:
				// Preserve the original numeric text exactly, including multi-segment values.
				if (!reader.HasValueSequence)
				{
					writer.WriteRawValue(reader.ValueSpan, skipInputValidation: true);
				}
				else
				{
					Span<byte> buffer = stackalloc byte[64];
					ReadOnlySequence<byte> sequence = reader.ValueSequence;
					if (sequence.Length <= buffer.Length)
					{
						sequence.CopyTo(buffer);
						writer.WriteRawValue(buffer[..(int)sequence.Length], skipInputValidation: true);
					}
					else
					{
						writer.WriteRawValue(sequence.ToArray(), skipInputValidation: true);
					}
				}
				break;
			case JsonTokenType.True: writer.WriteBooleanValue(true); break;
			case JsonTokenType.False: writer.WriteBooleanValue(false); break;
			case JsonTokenType.Null: writer.WriteNullValue(); break;
			default:
				throw new InvalidOperationException($"Unexpected token: {reader.TokenType}");
		}
	}
}

/// <summary>
/// Decorates an <see cref="IJsonDataSource"/> with a JSON upgrade pass. The upgraded bytes replace the
/// original sequence; position mapping back to the original stream is unavailable through the upgrade.
/// </summary>
public sealed class JsonUpgradingDataSource : IJsonDataSource
{
	private readonly IJsonDataSource _inner;
	private readonly JsonUpgradePipeline _pipeline;
	private readonly int _targetVersion;
	private readonly Func<ReadOnlySequence<byte>, int>? _versionReader;
	private readonly int? _sourceVersion;
	private readonly JsonUpgradeState? _state;

	private ReadOnlySequence<byte> _upgraded;
	private bool _processed;

	/// <summary>
	/// Creates the wrapper. The host may supply a delegate that reads the source version once at the start of
	/// the document, along with the sub-project upgrade state.
	/// </summary>
	public JsonUpgradingDataSource(
		IJsonDataSource inner,
		JsonUpgradePipeline pipeline,
		int targetVersion,
		Func<ReadOnlySequence<byte>, int>? versionReader = null,
		JsonUpgradeState? state = null)
	{
		_inner = inner;
		_pipeline = pipeline;
		_targetVersion = targetVersion;
		_versionReader = versionReader;
		_state = state;
	}

	/// <summary>
	/// Creates the wrapper with a source version the host already read outside the upgrader.
	/// </summary>
	public JsonUpgradingDataSource(
		IJsonDataSource inner,
		JsonUpgradePipeline pipeline,
		int targetVersion,
		int sourceVersion,
		JsonUpgradeState? state = null)
	{
		_inner = inner;
		_pipeline = pipeline;
		_targetVersion = targetVersion;
		_sourceVersion = sourceVersion;
		_state = state;
	}

	/// <inheritdoc/>
	public ReadOnlySequence<byte> GetSequence()
	{
		if (!_processed)
		{
			_upgraded = Upgrade(_inner.GetSequence());
			_processed = true;
		}
		return _upgraded;
	}

	/// <inheritdoc/>
	public async ValueTask<ReadOnlySequence<byte>> GetSequenceAsync(CancellationToken cancellationToken = default)
	{
		if (!_processed)
		{
			ReadOnlySequence<byte> source = await _inner.GetSequenceAsync(cancellationToken).ConfigureAwait(false);
			_upgraded = Upgrade(source);
			_processed = true;
		}
		return _upgraded;
	}

	/// <inheritdoc/>
	/// <remarks>The upgrade rewrites the byte stream, so positions cannot be mapped back to the source.</remarks>
	public long MapToInputPosition(long processedPosition) => -1;

	private ReadOnlySequence<byte> Upgrade(ReadOnlySequence<byte> source)
	{
		int version = _sourceVersion ?? _versionReader?.Invoke(source) ?? 0;
		if (version >= _targetVersion)
			return source;

		using MemoryStream stream = new();
		using (Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = false }))
		{
			JsonUpgradeContext context = new() { SourceVersion = version, TargetVersion = _targetVersion, State = _state };
			_pipeline.Process(source, writer, context);
			writer.Flush();
		}
		return new ReadOnlySequence<byte>(stream.ToArray());
	}
}
