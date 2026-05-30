using RhythmBase.RhythmDoctor.Components;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that occurs in a room.
/// </summary>
public interface IRoomEvent : IBaseEvent
{
	/// <summary>
	/// Gets or sets the room associated with the event.
	/// </summary>
	Room Rooms { get; set; }
}
