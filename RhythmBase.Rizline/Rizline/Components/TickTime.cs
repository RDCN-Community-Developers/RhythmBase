namespace RhythmBase.Rizline.Components;

/// <summary>
/// Represents a point in time measured in ticks, used for Rizline event timing.
/// </summary>
public struct TickTime : ITickTime<TickTime>
{
	/// <summary>
	/// The time span equivalent of this tick value.
	/// </summary>
	public TimeSpan TimeSpan { get; }
	/// <summary>
	/// The raw tick value.
	/// </summary>
	public float Tick { get; }

	/// <summary>
	/// Compares this tick time to another by their tick values.
	/// </summary>
	/// <param name="other">The other tick time to compare to.</param>
	/// <returns>An integer indicating the relative order.</returns>
	public int CompareTo(TickTime other)
	{
		return Tick.CompareTo(other.Tick);
	}

	/// <summary>
	/// Determines whether this tick time equals another by tick value.
	/// </summary>
	/// <param name="other">The other tick time to compare.</param>
	/// <returns><see langword="true"/> if the tick values are equal.</returns>
	public bool Equals(TickTime other)
	{
		return Tick.Equals(other.Tick);
	}
	/// <summary>
	/// Creates a new <see cref="TickTime"/> from the specified tick value.
	/// </summary>
	/// <param name="tick">The tick value.</param>
	public TickTime(float tick)
	{
		Tick = tick;
	}
}
/// <summary>
/// A range of tick time with optional start and end boundaries.
/// </summary>
public struct Range : ITickRange<TickTime>
{
	/// <summary>
	/// The inclusive start of the range, or <see langword="null"/> for unbounded.
	/// </summary>
	public TickTime? Start { get; }
	/// <summary>
	/// The inclusive end of the range, or <see langword="null"/> for unbounded.
	/// </summary>
	public TickTime? End { get; }
	/// <summary>
	/// Creates a new range with the specified start and end boundaries.
	/// </summary>
	/// <param name="start">The inclusive start boundary.</param>
	/// <param name="end">The inclusive end boundary.</param>
	public Range(TickTime? start, TickTime? end)
	{
		Start = start;
		End = end;
	}
	/// <summary>
	/// Determines whether the specified tick time falls within this range.
	/// </summary>
	/// <param name="b">The tick time to test.</param>
	/// <returns><see langword="true"/> if <paramref name="b"/> is within the range.</returns>
	public bool Contains(TickTime b)
	{
		throw new NotImplementedException();
	}
	/// <summary>
	/// Resolves the start and end tick times, substituting defaults where boundaries are unbounded.
	/// </summary>
	/// <param name="endTime">The fallback end time if the end boundary is unbounded.</param>
	/// <returns>A tuple of resolved start and end tick times.</returns>
	public (TickTime Start, TickTime End) GetStartAndEnd(TickTime endTime)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Returns the intersection of this range with another.
	/// </summary>
	/// <param name="other">The range to intersect with.</param>
	/// <returns>A new range representing the overlap.</returns>
	public ITickRange<TickTime> Intersect(ITickRange<TickTime> other)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Returns the union of this range with another.
	/// </summary>
	/// <param name="other">The range to union with.</param>
	/// <returns>A new range covering both ranges.</returns>
	public ITickRange<TickTime> Union(ITickRange<TickTime> other)
	{
		throw new NotImplementedException();
	}
#if NET8_0_OR_GREATER
	static ITickRange<TickTime> ITickRange<TickTime>.Infinity => new Range(null, null);
	static ITickRange<TickTime> ITickRange<TickTime>.Empty => new Range(new(), new());
#endif
}
