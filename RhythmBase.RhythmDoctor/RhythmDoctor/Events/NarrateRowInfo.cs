using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that narrates row information.
/// </summary>
[JsonObjectSerializable]
public record class NarrateRowInfo : BaseRowAction
{
	///<inheritdoc/>
	public override EventType Type => EventType.NarrateRowInfo;
	///<inheritdoc/>
	public override Tab Tab => Tab.Sounds;
	/// <summary>
	/// Gets or sets the type of narration information.
	/// </summary>
	public NarrateInfoType InfoType { get; set; } = NarrateInfoType.Online;
	/// <summary>
	/// Gets or sets the value indicating whether the narration is sound only.
	/// </summary>
	public bool SoundOnly { get; set; } = false;
	/// <summary>
	/// Gets or sets the beats to skip during narration.
	/// </summary>
	[JsonAlias("narrateSkipBeats")]
	public NarrateSkipBeat NarrateSkipBeat { get; set; } = NarrateSkipBeat.On;
	/// <summary>
	/// Gets or sets the custom pattern for the narration.
	/// </summary>
	[JsonCondition($"$&.{nameof(NarrateSkipBeat)} is RhythmBase.RhythmDoctor.{nameof(NarrateSkipBeat)}.{nameof(NarrateSkipBeat.Custom)}")]
	[JsonAlias("customPattern")]
	public PatternCollection Pattern { get; set; } = "------";
	/// <summary>
	/// Gets or sets a value indicating whether to skip unstable beats.
	/// </summary>
	public bool SkipsUnstable { get; set; } = false;
	/// <summary>  
	/// Gets or sets the custom player option for narrating row information.  
	/// </summary>  
	public PlayerType CustomPlayer { get; set; } = PlayerType.AutoDetect;
	/// <summary>
	/// Gets or sets the custom length of the row. The default value is 7.
	/// </summary>
	[JsonCondition($"$&.{nameof(CustomRowLength)} is 7")]
	public int CustomRowLength { get; set; } = 7;
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {InfoType}:{NarrateSkipBeat}";
}
