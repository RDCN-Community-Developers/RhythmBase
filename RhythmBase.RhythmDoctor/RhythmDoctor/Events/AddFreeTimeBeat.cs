using System.Diagnostics;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to add a free time beat.
/// </summary>
[JsonObjectSerializable]
public record class AddFreeTimeBeat : BaseBeat
{
	/// <summary>
	/// Gets or sets the hold duration of the beat.
	/// <remark>
	/// Must be non-negative value.
	/// </remark>
	/// </summary>
	public float Hold { get; set; }
	/// <summary>
	/// Gets or sets the pulse value of the beat.
	/// <remark>
	/// Must be value between -1 and 6, inclusive. 
	/// -1 is considered as the bomb beat.
	/// 6 is considered as the hit beat.
	/// </remark>
	/// </summary>
	[ValueRange<sbyte>(-1, 6)]
	public sbyte Pulse { get; set; }
	/// <inheritdoc/>
	public override EventType Type => EventType.AddFreeTimeBeat;}
