using RhythmBase.Global.Components.Easing;

namespace RhythmBase.Rizline.Events;

/// <summary>
/// Represents a BPM (beats per minute) change key point in a Rizline level.
/// </summary>
[JsonObjectSerializable]
public record class BpmShift : BaseEvent, IKeyPointEvent
{
	/// <summary>
	/// The BPM value at this key point.
	/// </summary>
	public float Value { get; set; }
	/// <summary>
	/// The event type, always <see cref="EventType.BpmShift"/>.
	/// </summary>
	public override EventType Type => EventType.BpmShift;
	/// <summary>
	/// Cumulative floor position at this key point.
	/// </summary>
	public float FloorPosition { get; set; }
	/// <summary>
	/// Ease type used when interpolating from this key point to the next.
	/// </summary>
	public EaseType Ease { get; set; }
	/// <summary>
	/// Duration of the interpolation from this key point to the next, in ticks.
	/// </summary>
	public float Duration { get; set; }
}