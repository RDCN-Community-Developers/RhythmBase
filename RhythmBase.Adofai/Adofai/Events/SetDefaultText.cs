using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to set default text properties in the Adofai editor.  
/// </summary>  
[JsonObjectSerializable]
public class SetDefaultText : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.SetDefaultText;
	/// <summary>  
	/// Gets or sets the duration of the event.  
	/// </summary>  
	public float Duration { get; set; } = 1f;
	/// <summary>  
	/// Gets or sets the easing type for the event.  
	/// </summary>  
	public EaseType Ease { get; set; }
	/// <summary>  
	/// Gets or sets the default text color.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(DefaultTextColor)} is not null")]
	public Color? DefaultTextColor { get; set; }
	/// <summary>  
	/// Gets or sets the default text shadow color.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(DefaultTextShadowColor)} is not null")]
	public Color? DefaultTextShadowColor { get; set; }
	/// <summary>  
	/// Gets or sets the position of the level title.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(LevelTitlePosition)} is not null")]
	public Point? LevelTitlePosition { get; set; }
	/// <summary>  
	/// Gets or sets the text for the level title.  
	/// </summary>  
	[JsonCondition($"!string.IsNullOrEmpty($&.{nameof(LevelTitleText)})")]
	public string LevelTitleText { get; set; } = string.Empty;
	/// <summary>  
	/// Gets or sets the text to display upon level completion.  
	/// </summary>  
	[JsonCondition($"!string.IsNullOrEmpty($&.{nameof(CongratsText)})")]
	public string CongratsText { get; set; } = string.Empty;
	/// <summary>  
	/// Gets or sets the text to display for a perfect score.  
	/// </summary>
	[JsonCondition($"!string.IsNullOrEmpty($&.{nameof(PerfectText)})")]
	public string PerfectText { get; set; } = string.Empty;
}
