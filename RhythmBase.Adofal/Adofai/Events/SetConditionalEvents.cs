namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event that sets conditional tags for various gameplay scenarios in ADOFAI.  
/// </summary>  
[JsonObjectSerializable]
public class SetConditionalEvents : BaseTileEvent, ISingleEvent
{	/// <inheritdoc/>
	public override EventType Type => EventType.SetConditionalEvents;	/// <summary>  
	/// Gets or sets the tag for a perfect hit.  
	/// </summary>  
	public string PerfectTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a normal hit.  
	/// </summary>  
	public string HitTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for an early perfect hit.  
	/// </summary>  
	public string EarlyPerfectTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a late perfect hit.  
	/// </summary>  
	public string LatePerfectTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a barely hit.  
	/// </summary>  
	public string BarelyTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a very early hit.  
	/// </summary>  
	public string VeryEarlyTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a very late hit.  
	/// </summary>  
	public string VeryLateTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a missed hit.  
	/// </summary>  
	public string MissTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a hit that was too early.  
	/// </summary>  
	public string TooEarlyTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a hit that was too late.  
	/// </summary>  
	public string TooLateTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a loss condition.  
	/// </summary>  
	public string LossTag { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag for a checkpoint event.  
	/// </summary>  
	public string OnCheckpointTag { get; set; } = string.Empty;
}
