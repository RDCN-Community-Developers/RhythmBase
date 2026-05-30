namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents a pause event in the ADOFAI level.  
/// </summary>  
[JsonObjectSerializable]
public class Pause : BaseTileEvent, ISingleEvent
{
	/// <inheritdoc/>  
	public override EventType Type => EventType.Pause;
	/// <summary>  
	/// Gets or sets the duration of the pause in seconds.  
	/// </summary>  
	public float Duration { get; set; } = 1f;
	/// <summary>  
	/// Gets or sets the number of countdown ticks during the pause.  
	/// </summary>  
	public int CountdownTicks { get; set; } = 0;
	/// <summary>  
	/// Gets or sets the direction of angle correction during the pause.  
	/// </summary>  
	[JsonAlias("angleCorrectionDir")]
	public AngleCorrectionDirection AngleCorrectionDirection { get; set; } = AngleCorrectionDirection.Backward;
}
