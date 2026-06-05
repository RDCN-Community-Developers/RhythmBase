namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents an event to set input configurations in the Adofai event system.
/// </summary>
[JsonObjectSerializable]
public class SetInputEvent : BaseTaggedTileEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.SetInputEvent;

	/// <summary>
	/// Gets or sets the target input action for this event.
	/// </summary>
	public Target Target { get; set; } = Target.Any;

	/// <summary>
	/// Gets or sets the key state (pressed or released) for the target input.
	/// </summary>
	public KeyState State { get; set; } = KeyState.Down;

	/// <summary>
	/// Gets or sets a value indicating whether input should be ignored.
	/// </summary>
	[JsonCondition($"$&.{nameof(State)} is RhythmBase.Adofai.{nameof(KeyState)}.{nameof(KeyState.Down)}")]
	public bool IgnoreInput { get; set; } = false;

	/// <summary>
	/// Gets or sets the tag of the target event associated with this input configuration.
	/// </summary>
	public string TargetEventTag { get; set; } = string.Empty;
}

