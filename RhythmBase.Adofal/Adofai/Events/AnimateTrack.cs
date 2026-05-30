namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to animate a track in the game.  
/// </summary>  
[JsonObjectSerializable]
public class AnimateTrack : BaseTileEvent, ISingleEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.AnimateTrack;
	/// <summary>  
	/// Gets or sets the track animation type.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(TrackAnimation)} is not null")]
	public TrackAnimationType? TrackAnimation { get; set; }
	/// <summary>  
	/// Gets or sets the track disappear animation type.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(TrackDisappearAnimation)} is not null")]
	public TrackDisappearAnimationType? TrackDisappearAnimation { get; set; }
	/// <summary>  
	/// Gets or sets the number of beats ahead for the animation.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(TrackAnimation)} is not RhythmBase.Adofai.{nameof(TrackAnimationType)}.{nameof(TrackAnimationType.None)}")]
	public float BeatsAhead { get; set; } = 3;
	/// <summary>  
	/// Gets or sets the number of beats behind for the animation.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(TrackDisappearAnimation)} is not RhythmBase.Adofai.{nameof(TrackDisappearAnimationType)}.{nameof(TrackDisappearAnimationType.None)}")]
	public float BeatsBehind { get; set; }
}
