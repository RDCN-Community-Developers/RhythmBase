namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a pulse free time beat event.
/// </summary>
[JsonObjectSerializable]
public record class PulseFreeTimeBeat : BaseBeat
{
	/// <summary>
	/// Gets or sets the hold duration.
	/// </summary>
	/// <remarks>
	/// Must be a non-negative value. If it's greater than 0, the pulse will be held for the specified duration in seconds.
	/// </remarks>
	public float Hold { get; set; }
	/// <summary>
	/// Gets or sets the custom pulse value.
	/// </summary>
	/// <remarks>
	/// Must be an integer between 0 and 6, inclutive. This property is only applicable when the <see cref="Action"/> is set to <see cref="PulseAction.Custom"/>.
	/// </remarks>
	[JsonCondition($"$&.{nameof(Action)} is RhythmBase.RhythmDoctor.{nameof(PulseAction)}.{nameof(PulseAction.Custom)}")]
	public uint CustomPulse { get; set; }
	/// <summary>
	/// Gets or sets the action type.
	/// </summary>
	public PulseAction Action { get; set; } = PulseAction.Increment;
	///<inheritdoc/>
	public override EventType Type => EventType.PulseFreeTimeBeat;
	///<inheritdoc/>
	public override string ToString()
	{
		string Out = "";
		switch (Action)
		{
			case PulseAction.Increment:
				Out = ">";
				break;
			case PulseAction.Decrement:
				Out = "<";
				break;
			case PulseAction.Custom:
				Out = (CustomPulse + 1).ToString();
				break;
			case PulseAction.Remove:
				Out = "X";
				break;
		}
		return base.ToString() + $"{Out}";
	}
}
