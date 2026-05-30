using RhythmBase.Global.Components.Easing;

namespace RhythmBase.RhythmDoctor.Components.Timeline;

/// <summary>
/// Represents the state of a oneshot pulse in the timeline.
/// </summary>
public enum OneshotPulseState
{
	/// <summary>
	/// No pulse state.
	/// </summary>
	None,

	/// <summary>
	/// The pulse is freezing.
	/// </summary>
	Freezing,
	/// <summary>
	/// The pulse is frozen.
	/// </summary>
	Frozen,
	/// <summary>
	/// The pulse is unfreezing.
	/// </summary>
	Unfreezing,

	/// <summary>
	/// The pulse is burning.
	/// </summary>
	Burning,
	/// <summary>
	/// The pulse is burned.
	/// </summary>
	Burned,
	/// <summary>
	/// The pulse is unburning.
	/// </summary>
	Unburning,
}
/// <summary>
/// Abstract base record for beat timelines, representing common properties for all beat types.
/// </summary>
public abstract record class BeatTimeline
{
	/// <summary>
	/// Gets or sets the type of row for this timeline.
	/// </summary>
	public RowType RowType { get; internal set; }

	/// <summary>
	/// Gets or sets the curve representing the hit index over time.
	/// </summary>
	public Curve<int> HitIndex { get; internal set; }

	/// <summary>
	/// Gets or sets the curve indicating whether the beat is being held over time.
	/// </summary>
	public Curve<bool> IsHolding { get; internal set; }
}
/// <summary>
/// Represents a classic beat timeline, including beat index and pattern curves.
/// </summary>
public record class ClassicBeatTimeline : BeatTimeline
{
	/// <summary>
	/// Gets or sets the curve representing the beat index over time.
	/// </summary>
	public Curve<int> BeatIndex { get; internal set; }

	/// <summary>
	/// Gets or sets the curve representing the patterns for each beat.
	/// </summary>
	public Curve<Pattern[]> Patterns { get; internal set; }
}
/// <summary>
/// Represents a oneshot beat timeline, including pulse type, beat count, skipping, and pulse state curves.
/// </summary>
public record class OneshotBeatTimeline : BeatTimeline
{
	/// <summary>
	/// Gets or sets the curve representing the pulse shape type over time.
	/// </summary>
	public Curve<OneshotPulseShapeType> PulseType { get; internal set; }

	/// <summary>
	/// Gets or sets the curve representing the beat count over time.
	/// </summary>
	public Curve<int> BeatCount { get; internal set; }

	/// <summary>
	/// Gets or sets the curve indicating whether the beat is skipping over time.
	/// </summary>
	public Curve<bool> IsSkipping { get; internal set; }

	/// <summary>
	/// Gets or sets the curve representing the pulse state over time.
	/// </summary>
	public Curve<OneshotPulseState> PulseState { get; internal set; }
}
