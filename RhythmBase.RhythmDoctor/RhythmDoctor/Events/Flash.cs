using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a Flash event.
/// </summary>
[JsonObjectSerializable]
public record class Flash : BaseEvent
{
	/// <summary>
	/// Gets or sets the rooms associated with the flash event.
	/// </summary>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets the duration of the flash event.
	/// </summary>
	public DurationType Duration { get; set; } = DurationType.Short;
	///<inheritdoc/>
	public override EventType Type => EventType.Flash;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {Duration}";
}