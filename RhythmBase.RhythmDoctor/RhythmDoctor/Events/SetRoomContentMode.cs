using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the room content mode.
/// </summary>
[JsonObjectSerializable]
public record class SetRoomContentMode : BaseEvent
{
	/// <summary>
	/// Gets or sets the mode of the room content.
	/// </summary>
	public ContentMode Mode { get; set; } = ContentMode.Center;
	///<inheritdoc/>
	public override EventType Type => EventType.SetRoomContentMode;
	///<inheritdoc/>
	public override Tab Tab => Tab.Rooms;

	/// <summary>
	/// Gets the room associated with the event.
	/// </summary>
	[JsonIgnore]
	public Room Room => new SingleRoom(checked((byte)Y));
}
