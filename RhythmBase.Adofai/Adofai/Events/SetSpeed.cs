namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to set the speed in the Adofai level.  
/// </summary>  
[JsonObjectSerializable]
public class SetSpeed : BaseTaggedTileEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.SetSpeed;
	/// <summary>  
	/// Gets or sets the type of speed adjustment.  
	/// </summary>  
	public SpeedType SpeedType { get; set; } = SpeedType.Bpm;
	/// <summary>  
	/// Gets or sets the beats per minute (BPM) value.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(SpeedType)} is RhythmBase.Adofai.{nameof(Adofai. SpeedType)}.{nameof(SpeedType.Bpm)}")]
	[ValueRange<float>(0.001f, 10000f)]
	public float BeatsPerMinute { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets the BPM multiplier value.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(SpeedType)} is RhythmBase.Adofai.{nameof(Adofai. SpeedType)}.{nameof(SpeedType.Multiplier)}")]
	[ValueRange<float>(1e-07f, 128f)]
	public float BpmMultiplier { get; set; } = 1f;
}
