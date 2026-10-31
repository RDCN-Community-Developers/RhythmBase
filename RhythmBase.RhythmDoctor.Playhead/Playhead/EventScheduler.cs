using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// An event with the ordering information used while simulating.
/// </summary>
internal readonly struct ScheduledEvent
{
	internal ScheduledEvent(float tick, EventExecutionPhase phase, int y, int order, IBaseEvent e)
	{
		Tick = tick;
		Phase = phase;
		Y = y;
		Order = order;
		Event = e;
	}

	internal float Tick { get; }
	internal EventExecutionPhase Phase { get; }
	internal int Y { get; }
	internal int Order { get; }
	internal IBaseEvent Event { get; }
}

/// <summary>
/// Walks the events of a chart in execution order and feeds them to a playhead.
/// </summary>
internal sealed class EventScheduler
{
	private readonly ScheduledEvent[] _events;
	private int _cursor;

	internal EventScheduler(Chart chart)
	{
		int order = 0;
		_events = chart
			.Select(e =>
			{
				float tick = e.TickTime.Tick;
				// A positive condition duration defers the event only when a dynamic (non-constant)
				// conditional is involved; the game runs constant-condition events immediately.
				float conditionDelay = e.Condition.Duration;
				if (conditionDelay > 0f && HasDynamicCondition(e, chart))
					tick += conditionDelay;
				return new ScheduledEvent(tick, EventExecutionInfo.PhaseOf(e.Type), e.Y, order++, e);
			})
			.OrderBy(x => x.Tick)
			.ThenBy(x => x.Phase)
			.ThenBy(x => x.Y)
			.ThenBy(x => x.Order)
			.ToArray();
	}

	private EventScheduler(ScheduledEvent[] events, int cursor)
	{
		_events = events;
		_cursor = cursor;
	}

	internal bool IsFinished => _cursor >= _events.Length;

	internal int Count => _events.Length;

	internal int Cursor => _cursor;

	internal float NextTick => _cursor < _events.Length ? _events[_cursor].Tick : float.PositiveInfinity;

	internal IBaseEvent EventAt(int index) => _events[index].Event;

	/// <summary>
	/// Processes every event due at or before <paramref name="tick"/>.
	/// </summary>
	internal int ProcessUpTo(float tick, Playhead playhead)
	{
		int processed = 0;
		while (_cursor < _events.Length && _events[_cursor].Tick <= tick)
		{
			playhead.ProcessEvent(_events[_cursor].Event, _cursor);
			_cursor++;
			processed++;
		}
		return processed;
	}

	/// <summary>
	/// Processes every event strictly before <paramref name="tick"/>.
	/// </summary>
	internal int ProcessBefore(float tick, Playhead playhead)
	{
		int processed = 0;
		while (_cursor < _events.Length && _events[_cursor].Tick < tick)
		{
			playhead.ProcessEvent(_events[_cursor].Event, _cursor);
			_cursor++;
			processed++;
		}
		return processed;
	}

	/// <summary>
	/// Moves the cursor to the first event at or after <paramref name="tick"/> without touching
	/// module state (used by backward seeks).
	/// </summary>
	internal void SeekTo(float tick)
	{
		int i = 0;
		while (i < _events.Length && _events[i].Tick < tick)
			i++;
		_cursor = i;
	}

	internal EventScheduler Clone() => new(_events, _cursor);

	private static bool HasDynamicCondition(IBaseEvent e, Chart chart)
	{
		foreach (BaseConditional conditional in chart.Conditionals)
		{
			if (e.Condition[conditional] is null)
				continue;
			if (!IsConstant(conditional.Type))
				return true;
		}
		// The global 'o' (once) tag behaves like a TimesExecuted conditional.
		return e.Condition['o'] is not null;
	}

	private static bool IsConstant(BaseConditional.ConditionType type) =>
		type is BaseConditional.ConditionType.Language
			or BaseConditional.ConditionType.PlayerMode
			or BaseConditional.ConditionType.Narration
			or BaseConditional.ConditionType.Accessibility;
}

