using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an action to set the counting sound in the rhythm base.
/// </summary>
[JsonObjectSerializable]
public record class SetCountingSound : BaseRowAction
{
	/// <summary>
	/// Gets or sets the voice source for the counting sound.
	/// </summary>
	[JsonCondition($"$&.{nameof(VoiceSource)} != RhythmBase.RhythmDoctor.{nameof(CountingSoundVoiceSource)}.{nameof(CountingSoundVoiceSource.Custom)}")]
	public CountingSoundVoiceSource VoiceSource { get; set; } = CountingSoundVoiceSource.JyiCount;
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="SetCountingSound"/> is enabled.
	/// </summary>
	public bool Enabled { get; set; } = true;
	/// <summary>
	/// Gets or sets the subdivision offset for the counting sound.
	/// </summary>
	/// <remarks>
	/// Only effective when the counting sound is enabled and the parent row is a oneshot row.
	/// </remarks>
	[JsonCondition($"$&.{nameof(Enabled)} && $&.{nameof(Parent)}?.{nameof(Row.RowType)} is RhythmBase.RhythmDoctor.{nameof(RowType)}.{nameof(RowType.Oneshot)}")]
	public float SubdivOffset { get; set; } = 0.5f;
	/// <summary>
	/// Gets or sets the volume of the counting sound.
	/// </summary>
	/// <remarks>
	/// The percentage of the original volume.
	/// Must be a value between 0 and 200, inclusive.
	/// </remarks>
	public int Volume { get; set; } = 100;
	/// <summary>
	/// Gets or sets the list of sounds for the counting sound.
	/// </summary>
	[JsonCondition($"$&.{nameof(Enabled)} && $&.{nameof(VoiceSource)} == RhythmBase.RhythmDoctor.{nameof(CountingSoundVoiceSource)}.{nameof(CountingSoundVoiceSource.Custom)}")]
	public Audio[] Sounds { get; set; } = [
		new Audio(){Filename = "Jyi - ChineseCount1" },
		new Audio(){Filename = "Jyi - ChineseCount2" },
		new Audio(){Filename = "Jyi - ChineseCount3" },
		new Audio(){Filename = "Jyi - ChineseCount4" },
		new Audio(){Filename = "Jyi - ChineseCount5" },
		new Audio(){Filename = "Jyi - ChineseCount6" },
		new Audio(){Filename = "Jyi - ChineseCount7" }
		];
	/// <summary>
	/// Gets the type of the event.
	/// </summary>
	public override EventType Type => EventType.SetCountingSound;

	/// <summary>
	/// Gets the tab associated with the event.
	/// </summary>
	public override Tab Tab => Tab.Sounds;
}
