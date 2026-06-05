namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event that plays a sound in the Adofai level.  
/// </summary>  
[JsonObjectSerializable]
public class PlaySound : BaseTaggedTileEvent, IBeginningEvent
{
	/// <summary>  
	/// Gets the type of the event, which is <see cref="EventType.PlaySound"/>.  
	/// </summary>  
	public override EventType Type => EventType.PlaySound;
	/// <summary>  
	/// Gets or sets the name of the hitsound to be played.  
	/// </summary>  
	public HitSound Hitsound { get; set; } = HitSound.Kick;
	/// <summary>  
	/// Gets or sets the volume of the hitsound.  
	/// </summary>  
	public int HitsoundVolume { get; set; } = 100;
}
