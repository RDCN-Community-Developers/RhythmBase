using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a custom screen shake event in Rhythm Doctor.
/// </summary>
[JsonObjectSerializable]
public record class ShakeScreenCustom : BaseEvent, IRoomEvent, IDurationEvent
{
	///<inheritdoc/>
	public override EventType Type => EventType.ShakeScreenCustom;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);

	/// <summary>
	/// Gets or sets the type of shake effect.
	/// </summary>
	public ShakeType ShakeType { get; set; } = ShakeType.Normal;
	/// <summary>
	/// Gets or sets the duration of the event, in seconds or beats depending on the value of <see cref="UseBeats"/>.
	/// </summary>
	/// <remarks>The duration value must be a non-negative floating-point number. A value of zero indicates that the
	/// event has no duration.</remarks>
	public float Duration { get; set; }

	/// <summary>
	/// Gets or sets the amplitude of the shake effect.
	/// </summary>
	public float Amplitude { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the duration is measured in beats.
	/// </summary>
	public bool UseBeats { get; set; } = false;

	/// <summary>
	/// Gets or sets the frequency of the shake effect.
	/// Only used when <see cref="ShakeType"/> is not <see cref="ShakeType.Normal"/>.
	/// </summary>
	[JsonCondition($"$&.{nameof(ShakeType)} is not RhythmBase.RhythmDoctor.{nameof(ShakeType)}.{nameof(ShakeType.Normal)}")]
	public float Frequency { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the shake effect should fade out.
	/// Only used when <see cref="ShakeType"/> is <see cref="ShakeType.Smooth"/> or <see cref="ShakeType.Rotate"/>.
	/// </summary>
	[JsonCondition($"""
    $&.{nameof(ShakeType)} is 
    RhythmBase.RhythmDoctor.{nameof(ShakeType)}.{nameof(ShakeType.Smooth)} or
    RhythmBase.RhythmDoctor.{nameof(ShakeType)}.{nameof(ShakeType.Rotate)}
    """)]
	public bool FadeOut { get; set; } = false;
}
