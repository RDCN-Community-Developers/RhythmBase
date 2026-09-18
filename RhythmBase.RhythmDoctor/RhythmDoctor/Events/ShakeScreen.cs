using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that shakes the screen.
/// </summary>
[JsonObjectSerializable]
public record class ShakeScreen : BaseEvent, IRoomEvent
{
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets the shake level of the event.
	/// </summary>
	public StrengthType ShakeLevel { get; set; } = StrengthType.Medium;
	/// <summary>
	/// Gets or sets the type of shake effect to apply.
	/// </summary>
	public ShakeType ShakeType { get; set; } = ShakeType.Normal;
	/// <summary>
	/// Gets the type of the event.
	/// </summary>		
	public override EventType Type => EventType.ShakeScreen;

	/// <summary>
	/// Gets the tab associated with the event.
	/// </summary>
	public override Tab Tab => Tab.Actions;
}
