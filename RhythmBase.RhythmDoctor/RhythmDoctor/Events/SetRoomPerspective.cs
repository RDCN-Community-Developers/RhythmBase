using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents an event to set the room perspective.  
/// </summary>  
[JsonObjectSerializable]
public record class SetRoomPerspective : BaseEvent, IEaseEvent, ISingleRoomEvent
{
	/// <summary>  
	/// Gets or sets the corner positions of the room.  
	/// </summary>  
	/// <remarks>
	/// Percentage of the screen width and height.
	/// (0,0) is the bottom-left corner, (100,100) is the top-right corner.
	/// The order of the corners is bottom-left, bottom-right, top-left, top-right. Leave it null to keep the original corner positions.
	/// </remarks>
	[Tween]
	[JsonAlias("cornerPositions")]
	public Corner Corner { get; set; }
	///<inheritdoc/>
	public float Duration { get; set; }
	///<inheritdoc/>
	public EaseType Ease { get; set; }
	///<inheritdoc/>
	public override EventType Type => EventType.SetRoomPerspective;
	///<inheritdoc/>
	public override Tab Tab => Tab.Rooms;
	/// <summary>  
	/// Gets the room associated with the event.  
	/// </summary>
	[JsonIgnore]
	public SingleRoom Room
	{
		get => new(checked((byte)Y));
		set => Y = value.Value;
	}
}
