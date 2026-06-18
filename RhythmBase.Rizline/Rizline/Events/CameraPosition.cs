using RhythmBase.Global.Components.Easing;

namespace RhythmBase.Rizline.Events;

/// <summary>
/// Represents a camera horizontal position key point in a Rizline level.
/// </summary>
[JsonObjectSerializable]
public record class CameraPosition : BaseEvent, IKeyPointEvent
{
	/// <summary>
	/// Cumulative floor position at this key point.
	/// </summary>
	public float FloorPosition { get; set; }
	/// <summary>
	/// The event type, always <see cref="EventType.CameraPosition"/>.
	/// </summary>
	public override EventType Type => EventType.CameraPosition;
	/// <summary>
	/// Ease type used when interpolating from this key point to the next.
	/// </summary>
	[JsonAlias("easeType")]
	public EaseType Ease { get; set; }
	/// <summary>
	/// Duration of the interpolation from this key point to the next, in ticks.
	/// </summary>
	public float Duration { get; set; }
	/// <summary>
	/// The horizontal position value at this key point.
	/// </summary>
	public float Value { get; set; }
}