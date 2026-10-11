using System.Collections.Generic;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// When an event is executed relative to a bar. The game stores this per event class through
/// <c>LevelEventInfoAttribute</c>; RhythmBase's event model does not carry it, so it is kept in
/// this central table (ported from the decompiled editor).
/// </summary>
public enum EventExecutionPhase
{
	/// <summary>Executed while the prebar of the coming bar is running (beats, sounds, row setup).</summary>
	OnPrebar,
	/// <summary>Executed on the bar itself.</summary>
	OnBar,
}

/// <summary>
/// Central lookup of the execution phase of every <see cref="EventType"/>.
/// </summary>
public static class EventExecutionInfo
{
	private static readonly HashSet<EventType> _prebar =
	[
		EventType.AddClassicBeat,
		EventType.AddFreeTimeBeat,
		EventType.AddOneshotBeat,
		EventType.ChangePlayersRows,
		EventType.PlaySong,
		EventType.PlaySound,
		EventType.PulseFreeTimeBeat,
		EventType.SetBeatSound,
		EventType.SetBeatsPerMinute,
		EventType.SetClapSounds,
		EventType.SetCountingSound,
		EventType.SetCrotchetsPerBar,
		EventType.SetHeartExplodeInterval,
		EventType.SetHeartExplodeVolume,
	];

	/// <summary>
	/// Gets the execution phase of an event type.
	/// </summary>
	/// <param name="type">The event type.</param>
	/// <returns>The execution phase.</returns>
	public static EventExecutionPhase PhaseOf(EventType type) => _prebar.Contains(type) ? EventExecutionPhase.OnPrebar : EventExecutionPhase.OnBar;

	/// <summary>
	/// Gets whether the event type runs on the prebar.
	/// </summary>
	/// <param name="type">The event type.</param>
	/// <returns><see langword="true"/> when the event runs on the prebar.</returns>
	public static bool IsPrebar(EventType type) => _prebar.Contains(type);
}

