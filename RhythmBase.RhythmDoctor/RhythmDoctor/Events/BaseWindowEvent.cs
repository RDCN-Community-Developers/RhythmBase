using RhythmBase.RhythmDoctor.Converters;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents the base class for all window-related events.  
/// </summary>  
[JsonObjectHasSerializer(typeof(RDInstanceConverter.BaseWindowEvent<>))]
public abstract record class BaseWindowEvent : BaseEvent
{
	/// <inheritdoc/>
	public override Tab Tab => Tab.Windows;

	/// <summary>  
	/// Gets the target window for this event.  
	/// </summary>
	public int TargetWindow { get => Y; set => Y = value; }
}
