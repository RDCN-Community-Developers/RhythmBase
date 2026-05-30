using System.Diagnostics;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to add a free time beat.
/// </summary>
[JsonObjectSerializable]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
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
	/// Must be value between 0 and 6, inclusive. 6 is considered as the hit beat.
	/// </remark>
	/// </summary>
	public byte Pulse { get; set; }
	/// <inheritdoc/>
	public override EventType Type => EventType.AddFreeTimeBeat;
	/// <inheritdoc/>
	public override string ToString() => base.ToString() + $" {Pulse + 1}";
	private string GetDebuggerDisplay() => ToString();
}
