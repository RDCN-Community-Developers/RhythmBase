using RhythmBase.Global.Components;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// One simulated hit input.
/// </summary>
public readonly struct HitRecord
{
	/// <summary>Gets the time of the hit.</summary>
	public float Tick { get; init; }
	/// <summary>Gets the row the hit belongs to (<c>-1</c> for any row).</summary>
	public sbyte Row { get; init; }
	/// <summary>Gets the hit result.</summary>
	public HitResult Result { get; init; }
}

/// <summary>
/// The hit state at a point in time.
/// </summary>
public readonly struct HitSnapshot
{
	/// <summary>Gets the hits that occurred up to the snapshot time.</summary>
	public IReadOnlyList<HitRecord> Hits { get; init; }
}

