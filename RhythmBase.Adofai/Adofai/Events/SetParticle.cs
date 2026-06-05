using RhythmBase.Global.Components.Easing;

namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents an event to set particle effects in the Adofai event system.
/// </summary>
[JsonObjectSerializable]
public class SetParticle : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <summary>
	/// Gets the type of the event.
	/// </summary>
	public override EventType Type => EventType.SetParticle;

	/// <summary>
	/// Gets or sets the duration of the particle effect.
	/// </summary>
	public float Duration { get; set; } = 1f;
	/// <summary>
	/// Gets or sets the tag associated with the particle effect.
	/// </summary>
	public string Tag { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the target mode for the particle effect.
	/// </summary>
	[JsonCondition($"$&.{nameof(TargetMode)} is not null")]
	public TargetMode? TargetMode { get; set; } = Adofai. TargetMode.Start;
	/// <summary>
	/// Gets or sets the easing type for the particle effect.
	/// </summary>
	public EaseType Ease { get; set; }
}