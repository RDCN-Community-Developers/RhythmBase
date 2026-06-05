using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents the Free Roam Twirl event in the ADOFAI editor.  
/// </summary>  
[JsonObjectSerializable]
public class FreeRoamTwirl : BaseTileEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.FreeRoamTwirl;
	/// <summary>  
	/// Gets or sets the position associated with the Free Roam Twirl event.  
	/// </summary>  
	public PointN Position { get; set; } = new(1, 0);
}
