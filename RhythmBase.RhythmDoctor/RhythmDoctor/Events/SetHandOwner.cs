using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the owner of a hand in a room.
/// </summary>
[JsonObjectSerializable]
public record class SetHandOwner : BaseEvent, IRoomEvent
{
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets the hand associated with the event.
	/// </summary>
	public PlayerHand Hand { get; set; } = PlayerHand.Right;
	/// <summary>
	/// Gets or sets the character associated with the event.
	/// </summary>
	public GameCharacter Character { get; set; } = GameCharacter.Player;
	///<inheritdoc/>
	public override EventType Type => EventType.SetHandOwner;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
}
