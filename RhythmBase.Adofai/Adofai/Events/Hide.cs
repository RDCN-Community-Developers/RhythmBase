namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event that hides specific elements in the level.  
/// </summary>  
[JsonObjectSerializable]
public class Hide : BaseTileEvent, ISingleEvent
{
	/// <inheritdoc/>  
	public override EventType Type => EventType.Hide;
	/// <summary>  
	/// Gets or sets a value indicating whether the judgment should be hidden.  
	/// </summary>  
	public bool HideJudgment { get; set; } = false;
	/// <summary>  
	/// Gets or sets a value indicating whether the tile icon should be hidden.  
	/// </summary>  
	public bool HideTileIcon { get; set; } = false;
}
