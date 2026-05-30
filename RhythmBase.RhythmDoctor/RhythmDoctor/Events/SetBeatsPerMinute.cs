namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the beats per minute (BPM) in the rhythm base.
/// </summary>
[JsonObjectSerializable]
public record class SetBeatsPerMinute : BaseBeatsPerMinute
{
	/// <inheritdoc/>
	public override float BeatsPerMinute { get => base.BeatsPerMinute; set => base.BeatsPerMinute = value; }
	/// <inheritdoc/>
	public override EventType Type => EventType.SetBeatsPerMinute;

	/// <inheritdoc/>
	public override Tab Tab => Tab.Sounds;

	/// <inheritdoc/>
	public override string ToString() => base.ToString() + $" BPM:{BeatsPerMinute}";
}
