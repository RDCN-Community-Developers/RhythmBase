namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// The kind of a simulated object state exposed by <see cref="PlayheadSnapshot.Objects"/>.
/// </summary>
public enum PlayheadObjectKind
{
	/// <summary>The conductor (time, BPM, play style).</summary>
	Conductor,
	/// <summary>A room.</summary>
	Room,
	/// <summary>A room camera.</summary>
	Camera,
	/// <summary>A row (track).</summary>
	Row,
	/// <summary>A text decoration.</summary>
	TextDecoration,
	/// <summary>A sprite decoration.</summary>
	SpriteDecoration,
	/// <summary>A VFX layer set.</summary>
	Vfx,
	/// <summary>A game window.</summary>
	Window,
	/// <summary>Audio playback.</summary>
	Audio,
	/// <summary>Gameplay (beats, heart).</summary>
	Gameplay,
	/// <summary>Variables.</summary>
	Variables,
	/// <summary>Text / UI.</summary>
	Text,
	/// <summary>A dialogue.</summary>
	Dialogue,
	/// <summary>A status sign.</summary>
	StatusSign,
	/// <summary>A floating text.</summary>
	FloatingText,
	/// <summary>Hit records.</summary>
	Hit,
}

/// <summary>
/// One object's state at a point in time, addressed by id and kind.
/// </summary>
public readonly struct PlayheadObjectSnapshot
{
	/// <summary>Gets the object id (stable within a level, e.g. a decoration id or <c>room0</c>).</summary>
	public string Id { get; init; }
	/// <summary>Gets the object kind.</summary>
	public PlayheadObjectKind Kind { get; init; }
	/// <summary>Gets the typed state object (for example a <see cref="DecorationSnapshot"/>, <see cref="RoomSnapshot"/>, ...).</summary>
	public object State { get; init; }
}
