namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents a blend decoration action.  
/// </summary>  
[JsonObjectSerializable]
public record class Blend : BaseDecorationAction
{
	///<inheritdoc/>
	public override EventType Type => EventType.Blend;
	///<inheritdoc/>
	public override Tab Tab => Tab.Decorations;
	/// <summary>  
	/// Gets or sets the type of blend effect to apply.  
	/// </summary>  
	public BlendType BlendType { get; set; } = BlendType.None;
}