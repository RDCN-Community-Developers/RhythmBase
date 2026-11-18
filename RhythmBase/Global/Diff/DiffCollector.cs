using System.Collections;
using System.Text.Json;

namespace RhythmBase.Global.Diff;

/// <summary>
/// Compares two event instances and reports property-level differences.
/// </summary>
public interface IEventDiffer
{
	/// <summary>Compares two instances, appending differences to <paramref name="collector"/>.</summary>
	void Diff(object? left, object? right, DiffCollector collector);
}

/// <summary>
/// Base class for generated, strongly typed event differs.
/// </summary>
/// <typeparam name="T">The event type.</typeparam>
public abstract class EventDiffer<T> : IEventDiffer
{
	void IEventDiffer.Diff(object? left, object? right, DiffCollector collector)
	{
		if (left is null && right is null)
			return;
		if (left is null || right is null)
		{
			collector.Add("", left, right);
			return;
		}
		DiffCore((T)left, (T)right, collector);
	}
	/// <summary>Compares two non-null instances.</summary>
	protected abstract void DiffCore(T left, T right, DiffCollector collector);
}

/// <summary>
/// Collects differences produced while walking an object graph.
/// </summary>
public sealed class DiffCollector
{
	private readonly List<DiffPropertyChange> _changes;
	private readonly string _prefix;
	private readonly string _modelPrefix;
	private readonly Func<Type, IEventDiffer?>? _resolver;
	private readonly Func<object?, string?>? _canonicalizer;
	private readonly bool _includeModelPaths;
	private readonly int _depth;
	private const int MaxDepth = 64;
	/// <summary>Initializes a new instance.</summary>
	/// <param name="resolver">Resolves a generated differ for a runtime type, or <see langword="null"/> to skip deep comparison.</param>
	/// <param name="canonicalizer">
	/// Produces a stable, representation-based string for a value, or <see langword="null"/> when the value has no
	/// canonical form. Used as a fallback so reference types without content equality (for example loaded file
	/// handles) are not reported as changed when their serialized form is identical.
	/// </param>
	public DiffCollector(Func<Type, IEventDiffer?>? resolver = null, Func<object?, string?>? canonicalizer = null)
		: this([], "", "", resolver, canonicalizer, false, 0) { }
	/// <summary>Initializes a new instance that optionally records CLR model paths.</summary>
	/// <param name="resolver">Resolves a generated differ for a runtime type, or <see langword="null"/> to skip deep comparison.</param>
	/// <param name="canonicalizer">Produces a stable, representation-based string for a value.</param>
	/// <param name="includeModelPaths">Whether to populate <see cref="DiffPropertyChange.ModelPath"/> alongside serialization paths.</param>
	public DiffCollector(Func<Type, IEventDiffer?>? resolver, Func<object?, string?>? canonicalizer, bool includeModelPaths)
		: this([], "", "", resolver, canonicalizer, includeModelPaths, 0) { }
	private DiffCollector(List<DiffPropertyChange> changes, string prefix, string modelPrefix, Func<Type, IEventDiffer?>? resolver, Func<object?, string?>? canonicalizer, bool includeModelPaths, int depth)
	{
		_changes = changes;
		_prefix = prefix;
		_modelPrefix = modelPrefix;
		_resolver = resolver;
		_canonicalizer = canonicalizer;
		_includeModelPaths = includeModelPaths;
		_depth = depth;
	}
	/// <summary>Gets the recorded changes.</summary>
	public IReadOnlyList<DiffPropertyChange> Changes => _changes;
	/// <summary>Gets a value indicating whether any change was recorded.</summary>
	public bool HasChanges => _changes.Count > 0;
	/// <summary>Records a change at <paramref name="path"/> using the same value for the model path.</summary>
	public void Add(string path, object? oldValue, object? newValue)
		=> Add(path, path, oldValue, newValue);
	/// <summary>Records a change at the serialization path and CLR model path.</summary>
	public void Add(string path, string modelPath, object? oldValue, object? newValue)
		=> _changes.Add(new DiffPropertyChange
		{
			Path = Compose(_prefix, path),
			ModelPath = _includeModelPaths ? Compose(_modelPrefix, modelPath) : null,
			OldValue = oldValue,
			NewValue = newValue,
		});
	private static string Compose(string prefix, string path)
	{
		if (path.Length == 0)
			return prefix.TrimEnd('.');
		if (path[0] == '[')
			return prefix.TrimEnd('.') + path;
		return prefix + path;
	}
	private static string ComposePrefix(string prefix, string path)
	{
		if (path.Length == 0)
			return prefix;
		if (path[0] == '[')
			return prefix.TrimEnd('.') + path + ".";
		return prefix + path + ".";
	}
	/// <summary>Compares two values and records differences recursively.</summary>
	public void Compare<T>(string path, T left, T right) => CompareValue(path, path, left, right);
	/// <summary>Compares two values using separate serialization and CLR model paths.</summary>
	public void Compare<T>(string path, string modelPath, T left, T right) => CompareValue(path, modelPath, left, right);
	private void CompareValue(string path, string modelPath, object? left, object? right)
	{
		if (_depth >= MaxDepth)
		{
			if (!Equals(left, right))
				Add(path, modelPath, left, right);
			return;
		}
		if (ReferenceEquals(left, right))
			return;
		if (left is null || right is null)
		{
			Add(path, modelPath, left, right);
			return;
		}
		if (left is string || right is string)
		{
			if (!left.Equals(right))
				Add(path, modelPath, left, right);
			return;
		}
		Type type = left.GetType();
		if (type == right.GetType() && _resolver?.Invoke(type) is IEventDiffer differ)
		{
			differ.Diff(left, right, new DiffCollector(_changes, ComposePrefix(_prefix, path), ComposePrefix(_modelPrefix, modelPath), _resolver, _canonicalizer, _includeModelPaths, _depth + 1));
			return;
		}
		// Prefer the serialized form for types that have a canonical representation (including
		// containers like SoundCollection). This avoids reference/count noise from resolved assets.
		if (type == right.GetType() && _canonicalizer is not null)
		{
			string? leftText = _canonicalizer(left);
			if (leftText is not null)
			{
				string? rightText = _canonicalizer(right);
				if (leftText == rightText)
					return;
				Add(path, modelPath, left, right);
				return;
			}
		}
		if (left is IEnumerable enumerableLeft && right is IEnumerable enumerableRight)
		{
			CompareSequence(path, modelPath, enumerableLeft, enumerableRight);
			return;
		}
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>) && type == right.GetType())
		{
			CompareValue(path + ".key", modelPath + ".key", type.GetProperty("Key")?.GetValue(left), type.GetProperty("Key")?.GetValue(right));
			CompareValue(path + ".value", modelPath + ".value", type.GetProperty("Value")?.GetValue(left), type.GetProperty("Value")?.GetValue(right));
			return;
		}
		if (!left.Equals(right))
			Add(path, modelPath, left, right);
	}
	/// <summary>
	/// Compares two sequences element by element. Exposed so hand-written differs can compare
	/// collection-like types without re-entering the differ for the collection itself.
	/// </summary>
	public void CompareSequence(string path, IEnumerable left, IEnumerable right)
		=> CompareSequence(path, path, left, right);
	/// <summary>Compares two sequences using separate serialization and CLR model paths.</summary>
	public void CompareSequence(string path, string modelPath, IEnumerable left, IEnumerable right)
	{
		List<object?> leftItems = [.. left.Cast<object?>()];
		List<object?> rightItems = [.. right.Cast<object?>()];
		if (leftItems.Count != rightItems.Count)
			Add(path, modelPath, leftItems.Count, rightItems.Count);
		int count = Math.Min(leftItems.Count, rightItems.Count);
		for (int i = 0; i < count; i++)
			CompareValue($"{path}[{i}]", $"{modelPath}[{i}]", leftItems[i], rightItems[i]);
	}
	/// <summary>Compares two unhandled-property bags.</summary>
	public static void CompareExtraData(DiffCollector collector, IReadOnlyDictionary<string, JsonElement>? left, IReadOnlyDictionary<string, JsonElement>? right)
		=> CompareExtraData(collector, left, right, "ExtraData");
	/// <summary>Compares two unhandled-property bags using the specified CLR model root.</summary>
	public static void CompareExtraData(DiffCollector collector, IReadOnlyDictionary<string, JsonElement>? left, IReadOnlyDictionary<string, JsonElement>? right, string modelPathRoot)
	{
		if (left is null && right is null)
			return;
		if (left is null || right is null)
		{
			collector.Add("<extra>", modelPathRoot, left, right);
			return;
		}
		HashSet<string> keys = [.. left.Keys];
		foreach (string key in right.Keys)
			keys.Add(key);
		foreach (string key in keys)
		{
			string modelPath = $"{modelPathRoot}[\"{EscapePathKey(key)}\"]";
			bool hasLeft = left.TryGetValue(key, out JsonElement leftValue);
			bool hasRight = right.TryGetValue(key, out JsonElement rightValue);
			if (hasLeft && hasRight)
			{
				if (leftValue.GetRawText() != rightValue.GetRawText())
					collector.Add("$" + key, modelPath, leftValue, rightValue);
			}
			else if (hasLeft)
				collector.Add("$" + key, modelPath, leftValue, null);
			else
				collector.Add("$" + key, modelPath, null, rightValue);
		}
	}
	private static string EscapePathKey(string key) => key.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
