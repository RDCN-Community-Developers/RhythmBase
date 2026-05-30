using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to show hands in a room.
/// </summary>
[JsonObjectSerializable]
public record class ShowHands : BaseEvent, IRoomEvent
{
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets the action to be performed.
	/// </summary>
	public ShowHandsAction Action { get; set; } = ShowHandsAction.Show;
	/// <summary>
	/// Gets or sets the hand of the player.
	/// </summary>
	public PlayerHand Hand { get; set; } = PlayerHand.Right;
	/// <summary>
	/// Gets or sets a value indicating whether the hands should be aligned.
	/// </summary>
	public bool Align { get; set; } = false;
	/// <summary>
	/// Gets or sets a value indicating whether the action should be instant.
	/// </summary>
	public bool ForceRaise { get; set; } = true;
	/// <summary>
	/// Gets or sets a value indicating whether the operation should be executed immediately.
	/// </summary>
	public bool Instant { get; set; } = true;
	/// <summary>
	/// Gets or sets the extent of the action.
	/// </summary>
	public ShowHandsExtent Extent { get; set; } = ShowHandsExtent.Full;
	///<inheritdoc/>
	public override EventType Type => EventType.ShowHands;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
}
