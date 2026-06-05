namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event that sets text in the Adofai level.  
/// </summary>  
[JsonObjectSerializable]
public class SetText : BaseTaggedTileEvent, IBeginningEvent
{	/// <inheritdoc/>
	public override EventType Type => EventType.SetText;
	/// <summary>  
	/// Gets or sets the text to be displayed in the level.  
	/// </summary>  
	[JsonAlias("decText")]
	public string DecorationText { get; set; } = string.Empty;	/// <summary>  
	/// Gets or sets the tag associated with the text event.  
	/// </summary>  
	public string Tag { get; set; } = string.Empty;
}
