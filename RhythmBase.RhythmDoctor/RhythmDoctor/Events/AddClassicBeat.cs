using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Extensions;
using System.Diagnostics;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to add a classic beat.
/// </summary>
[JsonObjectSerializable]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record class AddClassicBeat : BaseBeat
{
	/// <summary>
	/// Gets or sets the tick value.
	/// <remark>
	/// Must be non-negative value.
	/// </remark>
	/// </summary>
	public float Tick { get; set; } = 1f;
	/// <summary>
	/// Gets or sets the swing value.
	/// <remark>
	/// Must be value between 0 and 2, inclusive. 0 and 1 are considered as no swing.   
	/// </remark>
	/// </summary>
	[JsonCondition($"$&.{nameof(Swing)} is not 1f and not 0f")]
	public float Swing { get; set; }
	/// <summary>
	/// Gets or sets the hold value.
	/// <remark>
	/// Must be non-negative value. 0 is considered as no hold.
	/// </remark>
	/// </summary>
	[JsonCondition($"$&.{nameof(Hold)} != 0f")]
	public float Hold { get; set; }
	/// <summary>  
	/// Gets or sets the number of beats in a classic beat pattern.  
	/// <remark>
	/// Must be value between 1 and 7, inclusive. 7 is considered as no change to the beat pattern.
	/// </remark>
	/// </summary>   
	[JsonCondition($"$&.{nameof(Length)} != 7")]
	public int Length { get; set; } = 7;
	/// <summary>
	/// Gets or sets the audio content associated with this instance.
	/// </summary>
	public Audio? Sound { get; set; } = null;
	/// <summary>
	/// Gets or sets the classic beat pattern.
	/// <remark>
	/// Active only when Hold is not 0. 
	/// </remark>
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(Hold)} != 0f &&
		$&.{nameof(SetXs)} != RhythmBase.RhythmDoctor.{nameof(ClassicBeatPattern)}.{nameof(ClassicBeatPattern.NoChange)}
		""")]
	public ClassicBeatPattern SetXs { get; set; }
	/// <inheritdoc/>
	public override EventType Type => EventType.AddClassicBeat;
	/// <inheritdoc/>
	public override string ToString() => base.ToString() +
		$" {this.Pattern} {((Swing is 0.5f or 0f) ? "" : " Swing")}";
	private string GetDebuggerDisplay() => ToString();
}