using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents a screen tile action in the Adofai event system.  
/// </summary>  
[JsonObjectSerializable]
public class ScreenTile : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.ScreenTile;
	/// <summary>  
	/// Gets or sets the duration of the screen tile effect.  
	/// </summary>  
	public float Duration { get; set; } = 0;
	/// <summary>  
	/// Gets or sets the tile position for the screen tile effect.  
	/// </summary>  
	public PointN Tile { get; set; } = new(1, 1);
	/// <summary>  
	/// Gets or sets the easing type for the screen tile effect.  
	/// </summary>  
	public EaseType Ease { get; set; }
}
