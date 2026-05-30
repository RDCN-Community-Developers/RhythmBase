namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event that emits particles in the level.  
/// </summary>  
/// <remarks>  
/// This event is used to trigger particle effects during gameplay.  
/// </remarks>  
[JsonObjectSerializable]
public class EmitParticle : BaseTaggedTileEvent, IBeginningEvent
{
	/// <summary>  
	/// Gets the type of the event, which is <see cref="EventType.EmitParticle"/>.  
	/// </summary>  
	public override EventType Type => EventType.EmitParticle;

	/// <summary>  
	/// Gets or sets the tag associated with the particle emission.  
	/// </summary>  
	public string Tag { get; set; } = string.Empty;

	/// <summary>  
	/// Gets or sets the number of particles to emit.  
	/// </summary>  
	public int Count { get; set; } = 10;
}
