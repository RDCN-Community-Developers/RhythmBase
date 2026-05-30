namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the heart explode interval.
/// </summary>
[JsonObjectSerializable]
public record class SetHeartExplodeInterval : BaseEvent
{
	/// <summary>
	/// Gets or sets the type of interval.
	/// </summary>
	public HeartExplodeIntervalType IntervalType { get; set; } = HeartExplodeIntervalType.GatherAndCeil;
	/// <summary>
	/// Gets or sets the interval value.
	/// </summary>
	public float Interval { get; set; } = 1f;
	///<inheritdoc/>
	public override EventType Type => EventType.SetHeartExplodeInterval;
	///<inheritdoc/>
	public override Tab Tab => Tab.Sounds;
}