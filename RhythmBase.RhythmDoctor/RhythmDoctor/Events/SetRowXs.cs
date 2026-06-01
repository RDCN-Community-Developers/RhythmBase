using RhythmBase.RhythmDoctor.Components;
namespace RhythmBase.RhythmDoctor.Events;

/// <inheritdoc />
[JsonObjectSerializable]
public record class SetRowXs : BaseBeat
{
	/// <inheritdoc />
	public override EventType Type => EventType.SetRowXs;
	/// <summary>
	/// Gets or sets the pattern.
	/// </summary>
	public PatternCollection Pattern { get; set; } = "------";
	/// <summary>
	/// Gets or sets the synco beat.
	/// </summary>
	/// <remarks>
	/// Must be an integer between -1 and 5, inclusive. Set to -1 to disable syncopation.
	/// </remarks>
	public int SyncoBeat { get; set; } = -1;
	/// <summary>
	/// Gets or sets the synco swing.
	/// </summary>
	/// <remarks>
	/// Must be a value between 0 and 1, inclusive.
	/// This property is only applicable when the <see cref="SyncoBeat"/> is set to a value greater than or equal to 0.
	/// A value of 0 means no swing, while a value of 1 means maximum swing.
	/// </remarks>
	[JsonCondition($"$&.{nameof(SyncoBeat)} >= 0")]
	public float SyncoSwing { get; set; }
	/// <summary>
	/// Gets or sets the syncopation beat's cue style.
	/// </summary>
	[JsonCondition($"$&.{nameof(SyncoBeat)} >= 0")]
	public SetRowXsSyncoStyle SyncoStyle { get; set; } = SetRowXsSyncoStyle.Chirp;
	/// <summary>
	/// Gets or sets a value indicating whether to play the modifier sound.
	/// </summary>
	[JsonCondition($"$&.{nameof(SyncoBeat)} >= 0 && $&.{nameof(SyncoStyle)} is RhythmBase.RhythmDoctor.{nameof(SetRowXsSyncoStyle)}.{nameof(SetRowXsSyncoStyle.Chirp)}")]
	public bool SyncoPlayModifierSound { get; set; } = true;
	/// <summary>
	/// Gets or sets a value indicating whether the sound for turning off the SyncoPlay modifier is enabled.
	/// </summary>
	[JsonCondition($"$&.{nameof(SyncoBeat)} >= 0 && $&.{nameof(SyncoStyle)} is RhythmBase.RhythmDoctor.{nameof(SetRowXsSyncoStyle)}.{nameof(SetRowXsSyncoStyle.Chirp)}")]
	public bool SyncoPlayModifierOffSound { get; set; } = true;
	/// <summary>
	/// Gets or sets the synco volume.
	/// </summary>
	/// <remarks>
	/// Must be a value between 0 and 200, inclusive.
	/// This property is only applicable when the <see cref="SyncoBeat"/> is set to a value greater than or equal to 0.
	/// The percentage of the original volume. 
	/// </remarks>
	[JsonCondition($"$&.{nameof(SyncoBeat)} >= 0")]
	public int SyncoVolume { get; set; }
	/// <summary>
	/// Gets or sets the pitch adjustment value for synchronization operations.
	/// </summary>
	/// <remarks>
	/// Must be a value between 0 and 200, inclusive.
	/// The percentage of the original pitch.
	/// </remarks>
	[JsonCondition($"$&.{nameof(SyncoBeat)} >= 0")]
	public int SyncoPitch { get; set; } = 100;	
	/// <inheritdoc />
	public override string ToString() => base.ToString() + $" {Pattern}";
}
