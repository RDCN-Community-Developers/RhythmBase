using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that flips the screen in a room.
/// </summary>
[JsonObjectSerializable]
public record class FlipScreen : BaseEvent, IRoomEvent
{
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets a value indicating whether the screen should be flipped horizontally.
	/// </summary>
	public bool FlipX { get; set; } = false;
	/// <summary>
	/// Gets or sets a value indicating whether the screen should be flipped vertically.
	/// </summary>
	public bool FlipY { get; set; } = false;
	///<inheritdoc/>
	public override EventType Type => EventType.FlipScreen;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
	///<inheritdoc/>
	public override string ToString()
	{
		string result =
			FlipX
			? FlipY
				? "X"
				: "^v"
			: FlipY
				? "<>"
				: "";
		return base.ToString() + $" {result}";
	}
}
