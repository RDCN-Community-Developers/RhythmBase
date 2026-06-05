namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents the Hall of Mirrors event in the Adofai event system.  
/// </summary>  
[JsonObjectSerializable]
public class HallOfMirrors : BaseTaggedTileEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.HallOfMirrors;
	/// <summary>  
	/// Gets or sets a value indicating whether the Hall of Mirrors effect is enabled.  
	/// </summary>  
	public bool Enabled { get; set; } = true;
}
