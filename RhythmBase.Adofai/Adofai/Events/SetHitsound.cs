namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to set the hitsound in the ADOFAI level.  
/// </summary>  
[JsonObjectSerializable]
public class SetHitsound : BaseTileEvent, ISingleEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.SetHitsound;
	/// <summary>  
	/// Gets or sets the game sound associated with the hitsound.  
	/// </summary>  
	public GameSound GameSound { get; set; } = GameSound.Hitsound;
	/// <summary>  
	/// Gets or sets the custom hitsound file.  
	/// </summary>  
	public HitSound Hitsound { get; set; } = HitSound.Kick;
	/// <summary>  
	/// Gets or sets the volume of the hitsound.  
	/// </summary>  
	[ValueRange<int>(1, 100)]
	public int HitsoundVolume { get; set; } = 100;
}