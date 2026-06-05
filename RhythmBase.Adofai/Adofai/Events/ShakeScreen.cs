using RhythmBase.Global.Components.Easing;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents a screen shake event in the Adofai event system.  
/// </summary>  
[JsonObjectSerializable]
public class ShakeScreen : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.ShakeScreen;
	/// <summary>  
	/// Gets or sets the duration of the screen shake effect.  
	/// </summary>  
	public float Duration { get; set; } = 1f;
	/// <summary>  
	/// Gets or sets the strength of the screen shake effect.  
	/// </summary>  
	public float Strength { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets the intensity of the screen shake effect.  
	/// </summary>  
	public float Intensity { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets the easing type for the screen shake effect.  
	/// </summary>  
	public EaseType Ease { get; set; }
	/// <summary>  
	/// Gets or sets the fade-out duration for the screen shake effect.  
	/// </summary>  
	public bool FadeOut { get; set; } = true;
}
