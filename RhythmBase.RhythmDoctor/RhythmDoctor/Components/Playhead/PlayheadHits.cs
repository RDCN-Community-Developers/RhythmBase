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

/// <summary>
/// Supplies hit outcomes to <c>LastHit</c> conditions from a developer-provided timeline.
/// </summary>
internal sealed class HitModule : IPlayheadObject
{
	private readonly HitRecord[] _hits;

	internal HitModule(Playhead playhead, IReadOnlyList<HitRecord> timeline)
	{
		Playhead = playhead;
		_hits = [.. System.Linq.Enumerable.OrderBy(timeline, h => h.Tick)];
	}

	private HitModule(HitModule source)
	{
		Playhead = source.Playhead;
		_hits = source._hits;
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public string Id => "hits";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Hit;

	public PlayheadModules Module => PlayheadModules.Hits;

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } = [];

	public void MoveNext(IBaseEvent e)
	{
	}

	/// <summary>
	/// Gets the most recent hit result at or before <paramref name="tick"/> for a row.
	/// </summary>
	/// <param name="row">The row, or a negative value for any row.</param>
	/// <param name="tick">The current time.</param>
	/// <returns>The last result, or <see langword="null"/> when there is none.</returns>
	internal HitResult? LastHit(sbyte row, float tick)
	{
		for (int i = _hits.Length - 1; i >= 0; i--)
		{
			if (_hits[i].Tick > tick)
				continue;
			if (row < 0 || _hits[i].Row == row)
				return _hits[i].Result;
		}
		return null;
	}

	public object Snapshot()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		List<HitRecord> hits = [];
		foreach (HitRecord hit in _hits)
		{
			if (hit.Tick <= tick)
				hits.Add(hit);
		}
		return new HitSnapshot { Hits = hits };
	}

	public IReadOnlyList<PlayheadObjectSnapshot> Describe() => [new PlayheadObjectSnapshot { Id = Id, Kind = Kind, State = Snapshot() }];

	public IPlayheadObject Clone(Playhead playhead) => new HitModule(this) { Playhead = playhead };
}
