using RhythmBase.Global.Components.Easing;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to scale planets in the Adofai level.  
/// </summary>  
[JsonObjectSerializable]
public class ScalePlanets : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.ScalePlanets;
	/// <summary>  
	/// Gets or sets the duration of the scaling event.  
	/// </summary>  
	public float Duration { get; set; } = 1f;
	/// <summary>  
	/// Gets or sets the target planet(s) for the scaling event.  
	/// </summary>  
	public TargetPlanets TargetPlanet { get; set; } = TargetPlanets.FirePlanet;
	/// <summary>  
	/// Gets or sets the scale factor for the planets.  
	/// </summary>  
	[Tween]
	public float Scale { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets the easing type for the scaling transition.  
	/// </summary>  
	public EaseType Ease { get; set; }
}
