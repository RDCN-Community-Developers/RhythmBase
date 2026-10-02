using RhythmBase.Global.Diff;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Diff;

/// <summary>
/// Compares two Rhythm Doctor charts and reports added, removed and modified events.
/// </summary>
/// <remarks>
/// Events are matched by their event type together with their beat position. Within the same
/// type and position, events are matched in order; unmatched ones are reported as added or removed.
/// </remarks>
public static class ChartDiffer
{
	private const float TickEpsilon = 1e-4f;

	/// <summary>Compares two charts.</summary>
	/// <param name="left">The left-hand chart.</param>
	/// <param name="right">The right-hand chart.</param>
	/// <param name="options">
	/// Optional section flags. When <see langword="null"/> only the event sequence is compared.
	/// </param>
	/// <returns>The differences between the two charts.</returns>
	public static LevelDiff Diff(Chart left, Chart right, DiffOptions? options = null)
	{
		LevelDiff result = new();
		Dictionary<EventKey, List<IBaseEvent>> leftGroups = Group(left);
		Dictionary<EventKey, List<IBaseEvent>> rightGroups = Group(right);
		foreach (EventKey key in leftGroups.Keys.Union(rightGroups.Keys).OrderBy(k => k.Tick).ThenBy(k => k.Type))
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
			if (options.IncludeRows)
				DiffIndexed("Row", left.Rows.Count, i => left.Rows[i], i => right.Rows[i], right.Rows.Count, result);
			if (options.IncludeDecorations)
				DiffIndexed("Decoration", left.Decorations.Count, i => left.Decorations[i], i => right.Decorations[i], right.Decorations.Count, result);
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

	private readonly record struct EventKey(long Tick, EventType Type);

	private static Dictionary<EventKey, List<IBaseEvent>> Group(Chart chart)
	{
		Dictionary<EventKey, List<IBaseEvent>> map = [];
		foreach (IBaseEvent item in chart.ConcatAll())
		{
			EventKey key = new((long)Math.Round(item.TickTime.Tick / TickEpsilon), item.Type);
			if (!map.TryGetValue(key, out List<IBaseEvent>? list))
				map[key] = list = [];
			list.Add(item);
		}
		return map;
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

	private static string FormatPosition(IBaseEvent item) => $"Tick {item.TickTime.Tick:0.####}";
}
