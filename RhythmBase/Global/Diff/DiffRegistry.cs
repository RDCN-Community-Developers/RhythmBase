using System.Collections.Concurrent;

namespace RhythmBase.Global.Diff;

/// <summary>
/// Holds manually registered differs, used as a fallback when the source generator did not emit one
/// (for example generic types, or types whose element/field comparison needs custom semantics).
/// </summary>
public static class DiffRegistry
{
	private static readonly ConcurrentDictionary<Type, IEventDiffer> _map = new();
	/// <summary>Registers a differ for <paramref name="type"/>, replacing any previous registration.</summary>
	public static void Register(Type type, IEventDiffer differ) => _map[type] = differ;
	/// <summary>Registers a differ for <typeparamref name="T"/>, replacing any previous registration.</summary>
	public static void Register<T>(IEventDiffer differ) => Register(typeof(T), differ);
	/// <summary>Tries to resolve a manually registered differ for <paramref name="type"/>.</summary>
	public static bool TryGet(Type type, out IEventDiffer differ) => _map.TryGetValue(type, out differ!);
}
