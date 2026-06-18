namespace RhythmBase.Rizline.Events;

/// <summary>
/// Interface for events that have a cumulative floor position and easing.
/// </summary>
public interface IFloorPositionEvent : IBaseEvent, IEaseEvent
{
	/// <summary>
	/// Cumulative floor position (vertical offset) at this event.
	/// </summary>
	public float FloorPosition { get; set; }
}
