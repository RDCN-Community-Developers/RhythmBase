using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Events;
using RhythmBase.Global.Diff;

namespace RhythmBase.Adofai.Diff;

/// <summary>
/// Compares two ADOFAI levels and reports added, removed and modified events.
/// </summary>
/// <remarks>
/// Tile-bound events are matched by their event type and owning floor; time-authoritative events
/// are matched by their event type and tick. Within a matched group, events are paired in order.
/// </remarks>
public static class LevelDiffer
{
	private const float TickEpsilon = 1e-4f;
	private const int ScopeTile = 0;
	private const int ScopeEnd = 1;
	private const int ScopeFree = 2;

	/// <summary>Compares two levels.</summary>
	/// <param name="left">The left-hand level.</param>
	/// <param name="right">The right-hand level.</param>
	/// <param name="options">
	/// Optional section flags. When <see langword="null"/> only the event sequence is compared.
	/// </param>
	/// <returns>The differences between the two levels.</returns>
	public static LevelDiff Diff(Level left, Level right, DiffOptions? options = null)
	{
		LevelDiff result = new();
		Dictionary<EventKey, List<IBaseEvent>> leftGroups = Group(left);
		Dictionary<EventKey, List<IBaseEvent>> rightGroups = Group(right);
		foreach (EventKey key in leftGroups.Keys.Union(rightGroups.Keys)
			.OrderBy(k => k.Scope).ThenBy(k => k.Position).ThenBy(k => k.Type))
		{
			List<IBaseEvent> leftEvents = leftGroups.TryGetValue(key, out List<IBaseEvent>? a) ? a : [];
			List<IBaseEvent> rightEvents = rightGroups.TryGetValue(key, out List<IBaseEvent>? b) ? b : [];
			int matched = Math.Min(leftEvents.Count, rightEvents.Count);
			for (int i = 0; i < matched; i++)
				DiffEvent(leftEvents[i], rightEvents[i], result);
			for (int i = matched; i < leftEvents.Count; i++)
				result.Events.Add(CreateRemoved(leftEvents[i]));
			for (int i = matched; i < rightEvents.Count; i++)
				result.Events.Add(CreateAdded(rightEvents[i]));
		}
		if (options is not null)
		{
			if (options.IncludeSettings)
				DiffStructure("Settings", left.Settings, right.Settings, result);
			if (options.IncludeTiles)
				DiffIndexed("Tile", left.Count, i => left[i], i => right[i], right.Count, result);
		}
		return result;
	}

	private static void DiffIndexed(string label, int leftCount, Func<int, object> leftAt, Func<int, object> rightAt, int rightCount, LevelDiff result)
	{
		int matched = Math.Min(leftCount, rightCount);
		for (int i = 0; i < matched; i++)
			DiffStructure($"{label} {i}", leftAt(i), rightAt(i), result);
		for (int i = matched; i < leftCount; i++)
			result.Structures.Add(CreateStructure($"{label} {i}", DiffChangeKind.Removed, leftAt(i), null));
		for (int i = matched; i < rightCount; i++)
			result.Structures.Add(CreateStructure($"{label} {i}", DiffChangeKind.Added, null, rightAt(i)));
	}

	private static void DiffStructure(string label, object? left, object? right, LevelDiff result)
	{
		DiffCollector collector = new(EventDifferMap.Get, DiffValue.Canonicalize);
		IEventDiffer? differ = EventDifferMap.Get(left?.GetType() ?? right?.GetType() ?? typeof(object));
		if (differ is not null)
			differ.Diff(left, right, collector);
		else
			collector.Compare("", left, right);
		if (collector.HasChanges)
			result.Structures.Add(new DiffEventChange
			{
				TypeName = label,
				Kind = DiffChangeKind.Modified,
				Properties = collector.Changes,
				Left = left,
				Right = right,
			});
	}

	private static DiffEventChange CreateStructure(string label, DiffChangeKind kind, object? left, object? right) => new()
	{
		TypeName = label,
		Kind = kind,
		Left = left,
		Right = right,
	};

	private readonly record struct EventKey(EventType Type, int Scope, long Position);

	private static Dictionary<EventKey, List<IBaseEvent>> Group(Level level)
	{
		Dictionary<EventKey, List<IBaseEvent>> map = [];
		for (int i = 0; i < level.Count; i++)
			foreach (IBaseEvent item in level[i])
				Add(map, new EventKey(item.Type, ScopeTile, i), item);
		foreach (IBaseEvent item in level.End)
			Add(map, new EventKey(item.Type, ScopeEnd, 0), item);
		foreach (BaseEvent item in level.FreeEvents)
			Add(map, new EventKey(item.Type, ScopeFree, (long)Math.Round(item.TickTime.Tick / TickEpsilon)), item);
		return map;
	}

	private static void Add(Dictionary<EventKey, List<IBaseEvent>> map, EventKey key, IBaseEvent item)
	{
		if (!map.TryGetValue(key, out List<IBaseEvent>? list))
			map[key] = list = [];
		list.Add(item);
	}

	private static void DiffEvent(IBaseEvent left, IBaseEvent right, LevelDiff result)
	{
		DiffCollector collector = new(EventDifferMap.Get, DiffValue.Canonicalize);
		IEventDiffer? differ = EventDifferMap.Get(left.GetType());
		if (differ is not null)
			differ.Diff(left, right, collector);
		else
			collector.Compare("", left, right);
		if (collector.HasChanges)
			result.Events.Add(new DiffEventChange
			{
				Type = left.Type,
				TypeName = left.Type.ToString(),
				Kind = DiffChangeKind.Modified,
				Position = FormatPosition(left),
				Properties = collector.Changes,
				Left = left,
				Right = right,
			});
	}

	private static DiffEventChange CreateAdded(IBaseEvent item) => new()
	{
		Type = item.Type,
		TypeName = item.Type.ToString(),
		Kind = DiffChangeKind.Added,
		Position = FormatPosition(item),
		Right = item,
	};

	private static DiffEventChange CreateRemoved(IBaseEvent item) => new()
	{
		Type = item.Type,
		TypeName = item.Type.ToString(),
		Kind = DiffChangeKind.Removed,
		Position = FormatPosition(item),
		Left = item,
	};

	private static string FormatPosition(IBaseEvent item) => item switch
	{
		BaseTileEvent tileEvent when tileEvent.Parent is { } tile => $"Floor {tile.Index}",
		BaseEvent freeEvent => $"Tick {freeEvent.TickTime.Tick:0.####}",
		_ => "",
	};
}
