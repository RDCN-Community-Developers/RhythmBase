namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event that repeats specific actions in the level.  
/// </summary>  
[JsonObjectSerializable]
public class RepeatEvents : BaseTileEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.RepeatEvents;
	/// <summary>  
	/// Gets or sets the type of repetition for the event.  
	/// </summary>  
	public RepeatType RepeatType { get; set; } = RepeatType.Beat;
	/// <summary>  
	/// Gets or sets the number of repetitions for the event.  
	/// </summary>  
	public int Repetitions { get; set; } = 1;
	/// <summary>  
	/// Gets or sets the number of floors affected by the event.  
	/// </summary>  
	public int FloorCount { get; set; } = 1;
	/// <summary>  
	/// Gets or sets the interval between repetitions.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(RepeatType)} == RhythmBase.Adofai.{nameof(Adofai.RepeatType)}.{nameof(RepeatType.Beat)}")]
	public float Interval { get; set; } = 1f;
	/// <summary>  
	/// Gets or sets a value indicating whether the event should execute on the current floor.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(RepeatType)} == RhythmBase.Adofai.{nameof(Adofai.RepeatType)}.{nameof(RepeatType.Floor)}")]
	public bool ExecuteOnCurrentFloor { get; set; } = false;
	/// <summary>  
	/// Gets or sets the tag associated with the event.  
	/// </summary>  
	public string Tag { get; set; } = string.Empty;
}