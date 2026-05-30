using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents an event to show rooms.  
/// </summary>  
[JsonObjectSerializable]
public record class ShowRooms : BaseEvent, IEaseEvent, IRoomEvent
{
	/// <summary>  
	/// Gets or sets the height configuration for the room.  
	/// </summary>  
	[JsonAlias("heights")]
	public RoomHeight Height { get; set; } = new RoomHeight();
	/// <summary>  
	/// Gets or sets the room configuration associated with the height.  
	/// </summary>  
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>  
	/// Gets or sets the ease type for the event.  
	/// </summary>  
	public EaseType Ease { get; set; } = EaseType.InOutSine;
	/// <summary>  
	/// Gets or sets the duration of the transition.  
	/// </summary>  
	[JsonAlias("transitionTime")]
	public float Duration { get; set; } = 1;
	/// <summary>  
	/// Gets the type of the event.  
	/// </summary>  
	public override EventType Type => EventType.ShowRooms;

	/// <summary>  
	/// Gets the tab associated with the event.  
	/// </summary>  
	public override Tab Tab => Tab.Rooms;
}
