using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Converters;
namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents the base class for all tile-specific ADOFAI events.
/// </summary>
[JsonObjectHasSerializer(typeof(EventMemberConverterBaseTileEvent<>))]
public abstract class BaseTileEvent : BaseEvent
{
	/// <summary>
	/// Gets or sets the parent tile associated with this event.
	/// </summary>
	public Tile? Parent { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether the event is active.  
	/// </summary>  
	/// <value>  
	/// <c>true</c> if the event is active; otherwise, <c>false</c>.  
	/// </value>  
	public bool Active { get; set; } = true;
	/// <summary>
	/// Returns a string representation of the event type.
	/// </summary>
	/// <returns>A string that represents the event type.</returns>
	public override string ToString() => $"{Type}";
	internal int _floor = -1;
}
