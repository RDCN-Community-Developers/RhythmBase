namespace RhythmBase.Rizline.Events;

/// <summary>
/// Interface for key point events that have a value, floor position and easing.
/// </summary>
public interface IKeyPointEvent : IFloorPositionEvent
{
	/// <summary>
	/// The value at this key point (meaning depends on event type).
	/// </summary>
	public float Value { get; set; }
}