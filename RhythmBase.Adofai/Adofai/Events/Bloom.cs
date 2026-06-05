using RhythmBase.Global.Components.Easing;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents a Bloom event in the Adofai event system.  
/// </summary>  
[JsonObjectSerializable]
public class Bloom : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.Bloom;
	/// <summary>  
	/// Gets or sets a value indicating whether the bloom effect is enabled.  
	/// </summary>  
	public bool Enabled { get; set; } = true;
	/// <summary>  
	/// Gets or sets the threshold value for the bloom effect.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Enabled)}")]
	public int Threshold { get; set; } = 50;
	/// <summary>  
	/// Gets or sets the intensity of the bloom effect.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Enabled)}")]
	public int Intensity { get; set; } = 100;
	/// <summary>  
	/// Gets or sets the color of the bloom effect.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Enabled)}")]
	public Color Color { get; set; } = Color.White;
	/// <summary>  
	/// Gets or sets the duration of the bloom effect.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Enabled)}")]
	public float Duration { get; set; }
	/// <summary>  
	/// Gets or sets the easing type for the bloom effect.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Enabled)}")]
	public EaseType Ease { get; set; }
}
