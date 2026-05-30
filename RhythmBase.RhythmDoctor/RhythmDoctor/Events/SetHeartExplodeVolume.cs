namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the volume of the heart explosion sound.
/// </summary>
[JsonObjectSerializable]
public record class SetHeartExplodeVolume : BaseEvent, IBarBeginningEvent
{
	/// <summary>
	/// Gets or sets the volume of the heart explosion sound.
	/// </summary>
	public int Volume { get; set; }
	///<inheritdoc/>
	public override EventType Type => EventType.SetHeartExplodeVolume;
	///<inheritdoc/>
	public override Tab Tab => Tab.Sounds;
}
