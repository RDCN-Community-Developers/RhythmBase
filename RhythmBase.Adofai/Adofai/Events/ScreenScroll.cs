using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents a screen scroll action in the Adofai event system.  
/// </summary>  
[JsonObjectSerializable]
public class ScreenScroll : BaseTaggedTileEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.ScreenScroll;
	/// <summary>  
	/// Gets or sets the scroll size for the screen.  
	/// </summary>  
	public SizeN Scroll { get; set; } = new(0, 0);
}
