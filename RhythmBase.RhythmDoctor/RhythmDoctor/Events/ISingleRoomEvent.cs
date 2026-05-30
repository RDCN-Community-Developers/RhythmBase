using RhythmBase.RhythmDoctor.Components;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that occurs within a single room.
/// </summary>
public interface ISingleRoomEvent : IBaseEvent
{
	/// <summary>
	/// Gets or sets the room associated with the event.
	/// </summary>
	SingleRoom Room { get; set; }
}
