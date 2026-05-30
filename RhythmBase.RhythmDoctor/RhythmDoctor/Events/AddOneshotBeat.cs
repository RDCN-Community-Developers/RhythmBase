using RhythmBase.RhythmDoctor.Components;
using System.Diagnostics;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to add a one-shot beat.
/// </summary>
[JsonObjectSerializable]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record class AddOneshotBeat : BaseBeat
{
	/// <summary>
	/// Gets or sets the type of pulse.
	/// </summary>
	public OneshotPulseShapeType PulseType { get; set; }
	/// <summary>
	/// Gets or sets the number of subdivisions.
	/// <remark>
	/// Must be non-negative value. Active only when PulseType is Square or Triangle. 0 is considered as no subdivision.
	/// </remark>
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(Subdivisions)} > 0 &&(
		$&.{nameof(PulseType)}
		is RhythmBase.RhythmDoctor.{nameof(OneshotPulseShapeType)}.{nameof(OneshotPulseShapeType.Square)}
		or RhythmBase.RhythmDoctor.{nameof(OneshotPulseShapeType)}.{nameof(OneshotPulseShapeType.Triangle)}
		)
		""")]
	public byte Subdivisions { get; set; } = 1;
	/// <summary>
	/// Gets or sets a value indicating whether the subdivision sound is enabled.
	/// </summary>
	[JsonAlias("subdivSound")]
	[JsonCondition($"$&.{nameof(Subdivisions)} > 0")]
	public bool SubdivisionSound { get; set; }
	/// <summary>
	/// 
	/// </summary>
	[JsonAlias("subdivTickOverride")]
	[JsonCondition($"$&.{nameof(SubdivisionTickOverride)} > 0")]
	public float SubdivisionTickOverride { get; set; }
	/// <summary>
	/// Gets or sets the tick value.
	/// <remark>
	/// Must be non-negative value.
	/// </remark>
	/// </summary>
	public float Tick { get; set; } = 1f;
	/// <summary>
	/// Gets or sets the number of loops.
	/// <remark>
	/// Must be non-negative value. Active only when Loop is greater than 0. 0 is considered as no loop (Play once).  
	/// </remark>
	/// </summary>
	[JsonAlias("loops")]
	[JsonCondition($"$&.{nameof(Loop)} > 0")]
	public uint Loop { get; set; }
	/// <summary>
	/// Gets or sets the interval value.
	/// <remark>
	/// Must be non-negative value.
	/// Active only when Interval is greater than 0
	/// (0 is considered as no interval (Play continuously)
	/// and Skipshot is false, Hold is false, FreezeBurnMode is not Wave, or Loop is greater than 0.
	/// </remark>
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(Interval)} > 0 &&(
		$&.{nameof(Skipshot)} ||
		$&.{nameof(Hold)} ||
		$&.{nameof(FreezeBurnMode)} is not RhythmBase.RhythmDoctor.{nameof(OneshotType)}.{nameof(OneshotType.Wave)} ||
		$&.{nameof(Loop)} > 0)
		""")]
	public float Interval { get; set; } = 2;
	/// <summary>
	/// Gets or sets a value indicating whether to skip the shot.
	/// </summary>
	[JsonCondition($"$&.{nameof(Skipshot)}")]
	public bool Skipshot { get; set; }

	/// <summary>  
	/// Gets or sets a value indicating whether the hold action is enabled.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Hold)}")]
	public bool Hold { get; set; } = false;

	/// <summary>  
	/// Gets or sets a value indicating whether a sudden hold cue is triggered.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(Hold)}")]
	public HoldCue HoldCue { get; set; } = HoldCue.Auto;

	/// <summary>
	/// Gets or sets the freeze burn mode.
	/// </summary>
	[JsonCondition($"$&.{nameof(FreezeBurnMode)} is not RhythmBase.RhythmDoctor.{nameof(OneshotType)}.{nameof(OneshotType.Wave)}")]
	public OneshotType FreezeBurnMode { get; set; }
	/// <summary>
	/// Gets or sets the delay value.
	/// <remark>
	/// Must be non-negative value. Active only when FreezeBurnMode is Freezeshot. 0 is considered as default delay (0.5s).
	/// </remark>
	/// </summary>
	[JsonCondition($"$&.{nameof(FreezeBurnMode)} == RhythmBase.RhythmDoctor.{nameof(OneshotType)}.{nameof(OneshotType.Freezeshot)}")]
	public float Delay { get; set; } = 0f;
	/// <summary>
	/// Gets or sets the audio resource associated with this object.
	/// </summary>
	/// <remarks>The value may be null, indicating that no audio is currently assigned. Ensure that the audio
	/// resource is properly initialized before attempting playback or processing operations.</remarks>
	public Audio? Sound { get; set; }
	/// <inheritdoc/>
	public override EventType Type => EventType.AddOneshotBeat;
	/// <inheritdoc/>
	public override string ToString() => base.ToString() + $" {FreezeBurnMode} {PulseType}";
	private string GetDebuggerDisplay() => ToString();
}