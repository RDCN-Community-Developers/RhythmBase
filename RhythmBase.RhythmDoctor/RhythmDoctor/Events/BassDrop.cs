using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents a BassDrop event.  
/// </summary>  
[JsonObjectSerializable]
public record class BassDrop : BaseEvent, IRoomEvent
{
	/// <inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>  
	/// Gets or sets the strength of the BassDrop event.  
	/// </summary>  
	public StrengthType Strength { get; set; } = StrengthType.High;
	/// <inheritdoc/>
	public override EventType Type => EventType.BassDrop;

	/// <inheritdoc/>
	public override Tab Tab => Tab.Actions;

	/// <inheritdoc/>
	public override string ToString() => base.ToString() + $" {Strength}";
}