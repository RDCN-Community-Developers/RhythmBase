using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that fades a room.
/// </summary>
[JsonObjectSerializable]
public record class FadeRoom : BaseEvent, IEaseEvent, ISingleRoomEvent
{
	///<inheritdoc/>
	public EaseType Ease { get; set; }
	/// <summary>
	/// Gets or sets the opacity level for the fade effect.
	/// <remark>
	/// Must be a value between 0 and 100, inclusive, if specified. 0 and 100 are considered as fully transparent and fully opaque, respectively.
	/// </remark>
	/// </summary>
	[Tween]
	public int Opacity { get; set; } = 100;
	///<inheritdoc/>
	public float Duration { get; set; }
	///<inheritdoc/>
	public override EventType Type => EventType.FadeRoom;
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
