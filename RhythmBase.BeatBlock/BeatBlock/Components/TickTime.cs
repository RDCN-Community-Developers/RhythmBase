namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents a beat in the BeatBlock level format.
/// </summary>
public struct TickTime : ITickTime<TickTime>
{
    /// <summary>
    /// Gets the time span of the beat.
    /// </summary>
    public TimeSpan TimeSpan { get; }
    /// <summary>
    /// Gets the beat value.
    /// </summary>
    public float Tick { get; }
    /// <summary>
    /// Compares this instance to another <see cref="TickTime"/>.
    /// </summary>
    /// <param name="other">The other <see cref="TickTime"/> to compare to.</param>
    /// <returns>A value indicating the relative order of the instances.</returns>
    public int CompareTo(TickTime other)
    {
        return Tick.CompareTo(other.Tick);
    }
    /// <summary>
    /// Determines whether this instance equals another <see cref="TickTime"/>.
    /// </summary>
    /// <param name="other">The other <see cref="TickTime"/> to compare to.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(TickTime other)
    {
        return Tick.Equals(other.Tick);
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="TickTime"/> struct.
    /// </summary>
    /// <param name="beatOnly">The beat value.</param>
    public TickTime(float beatOnly)
    {
        TimeSpan = TimeSpan.Zero;
        Tick = beatOnly;
    }
}
/// <summary>
/// Represents a range of tick times.
/// </summary>
public struct Range : ITickRange<TickTime>
{
    /// <summary>
    /// Gets the start of the range.
    /// </summary>
    public TickTime? Start { get; }
    /// <summary>
    /// Gets the end of the range.
    /// </summary>
    public TickTime? End { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Range"/> struct.
    /// </summary>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    public Range(TickTime? start, TickTime? end)
    {
        Start = start;
        End = end;
    }
    /// <summary>
    /// Determines whether the range contains the specified tick time.
    /// </summary>
    /// <param name="b">The tick time to check.</param>
    /// <returns><see langword="true"/> if the range contains the specified tick time; otherwise, <see langword="false"/>.</returns>
    public bool Contains(TickTime b)
    {
        return Start?.CompareTo(b) <= 0 && End?.CompareTo(b) >= 0;
    }

    /// <inheritdoc/>
    public (TickTime Start, TickTime End) GetStartAndEnd(TickTime endTime)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ITickRange<TickTime> Intersect(ITickRange<TickTime> other)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ITickRange<TickTime> Union(ITickRange<TickTime> other)
    {
        throw new NotImplementedException();
	}
#if NET8_0_OR_GREATER
  static ITickRange<TickTime> ITickRange<TickTime>.Infinity => new Range(null, null);
	static ITickRange<TickTime> ITickRange<TickTime>.Empty => new Range(new(), new());
#endif
}