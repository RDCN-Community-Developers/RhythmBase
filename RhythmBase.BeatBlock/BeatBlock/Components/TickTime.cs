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
public struct BBRange : ITickRange<TickTime>
{
    public TickTime? Start { get; }
    public TickTime? End { get; }

    public BBRange(TickTime? start, TickTime? end)
    {
        Start = start;
        End = end;
    }
    public BBRange(float? start, float? end)
    {
        Start = start.HasValue ? new TickTime(start.Value) : null;
        End = end.HasValue ? new TickTime(end.Value) : null;
    }

    public bool Contains(TickTime b)
    {
        return Start?.CompareTo(b) <= 0 && End?.CompareTo(b) >= 0;
    }

    public (TickTime Start, TickTime End) GetStartAndEnd(TickTime endTime)
    {
        throw new NotImplementedException();
    }

    public ITickRange<TickTime> Intersect(ITickRange<TickTime> other)
    {
        throw new NotImplementedException();
    }

    public ITickRange<TickTime> Union(ITickRange<TickTime> other)
    {
        throw new NotImplementedException();
    }
}